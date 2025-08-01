using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Data.Entities;
using CloneEbay.Models;
using CloneEbay.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using CloneEbay.Interfaces;

namespace CloneEbay.Pages
{
    public class OrderModel : PageModel
    {
        private readonly IAddressService _addressService;
        private readonly CartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IShippingProvider _shippingProvider;

        public OrderModel(IAddressService addressService, CartService cartService, IOrderService orderService, IShippingProvider shippingProvider)
        {
            _addressService = addressService;
            _cartService = cartService;
            _orderService = orderService;
            _shippingProvider = shippingProvider;
        }

        public List<CartItem> CartItems { get; set; } = new();
        public decimal TotalPrice => CartItems.Sum(x => x.Price * x.Quantity);
        public List<Address> Addresses { get; set; } = new();

        [BindProperty]
        public int SelectedAddressId { get; set; }
        [BindProperty]
        public Address NewAddress { get; set; } = new();
        [BindProperty]
        public bool AddNewAddress { get; set; }
        [BindProperty]
        public string CouponCode { get; set; } = string.Empty;
        public string CouponError { get; set; } = string.Empty;
        public decimal CouponDiscountPercent { get; set; } = 0;
        public decimal DiscountedTotal { get; set; } = 0;
        public int? CouponId { get; set; } = null;

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");
            
            Addresses = await _addressService.GetUserAddressesAsync(userId.Value);
            CartItems = _cartService.GetCartItems();
            
            if (Addresses.Any(a => a.IsDefault == true))
                SelectedAddressId = Addresses.First(a => a.IsDefault == true).Id;
            
            // Coupon logic for GET (show discount if code in query)
            if (!string.IsNullOrEmpty(CouponCode))
            {
                await ApplyCouponAsync();
            }
            else
            {
                DiscountedTotal = TotalPrice;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAddAddressAsync()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");

            // Nếu địa chỉ mới là default, set các địa chỉ khác về not default
            if (NewAddress.IsDefault == true)
            {
                var allAddresses = await _addressService.GetUserAddressesAsync(userId.Value);
                foreach (var addr in allAddresses)
                {
                    if (addr.IsDefault == true)
                    {
                        addr.IsDefault = false;
                        await _addressService.UpdateAddressAsync(addr); // cần có hàm update
                    }
                }
            }

            NewAddress.UserId = userId.Value;
            var success = await _addressService.AddAddressAsync(NewAddress);
            
            if (!success)
            {
                ModelState.AddModelError("", "Không thể thêm địa chỉ. Vui lòng thử lại.");
                Addresses = await _addressService.GetUserAddressesAsync(userId.Value);
                return Page();
            }
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostPlaceOrderAsync()
        {
            var userId = GetUserId();
            if (userId == null)
                return RedirectToPage("/Auth/Login");
            
            CartItems = _cartService.GetCartItems();
            if (CartItems.Count == 0)
                return RedirectToPage("/Cart");
            
            // Validate address belongs to user
            var address = await _addressService.GetAddressByIdAsync(SelectedAddressId, userId.Value);
            if (address == null)
            {
                ModelState.AddModelError("", "Vui lòng chọn địa chỉ giao hàng.");
                Addresses = await _addressService.GetUserAddressesAsync(userId.Value);
                return Page();
            }
            
            // Create order using service
            var order = await _orderService.CreateOrderAsync(userId.Value, SelectedAddressId, CouponCode, CartItems);
            if (order == null)
            {
                ModelState.AddModelError("", "Không thể tạo đơn hàng hoặc mã giảm giá không hợp lệ. Vui lòng thử lại.");
                Addresses = await _addressService.GetUserAddressesAsync(userId.Value);
                return Page();
            }
            
            // Update coupon usage if used
            if (order.CouponId.HasValue)
            {
                await _orderService.UpdateCouponUsageAsync(order.CouponId.Value);
            }

            // --- TÍNH PHÍ GIAO HÀNG & TẠO VẬN ĐƠN ---
            string region = address.GetRegion();
            decimal shippingFee = CloneEbay.Services.ShippingFeeCalculator.Calculate(region);
            var shippingRequest = new CloneEbay.Models.ShippingRequestModel
            {
                OrderId = order.Id.ToString(),
                Address = address.GetFullAddress(),
                UserId = userId.Value.ToString(),
                Region = region
            };
            var shippingResult = await _shippingProvider.CreateShipmentAsync(shippingRequest);
            if (shippingResult.Success)
            {
                // Cập nhật trạng thái shipment (giả lập)
                await _shippingProvider.UpdateShipmentStatusAsync(shippingResult.ShipmentCode, "Created");
                // Có thể lưu shipmentCode vào TempData nếu muốn dùng ở Payment
                TempData["ShipmentCode"] = shippingResult.ShipmentCode;
            }
            else
            {
                // Xử lý lỗi tạo vận đơn nếu cần
                TempData["ShipmentError"] = shippingResult.Message;
            }

            _cartService.ClearCart();
            return RedirectToPage("/Payment", new { orderId = order.Id });
        }

        private async Task ApplyCouponAsync()
        {
            CouponDiscountPercent = 0;
            DiscountedTotal = TotalPrice;
            CouponId = null;
            if (!string.IsNullOrWhiteSpace(CouponCode))
            {
                var couponResult = await _orderService.ValidateAndApplyCouponAsync(CouponCode, CartItems);
                if (couponResult.IsValid)
                {
                    CouponDiscountPercent = couponResult.DiscountPercent;
                    CouponId = couponResult.CouponId;
                    DiscountedTotal = TotalPrice * (1 - (CouponDiscountPercent / 100));
                }
                else
                {
                    CouponError = couponResult.ErrorMessage ?? "Mã giảm giá không hợp lệ.";
                }
            }
        }

        private int? GetUserId()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    return userId;
            }
            return null;
        }
    }
} 