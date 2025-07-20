using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Data;
using CloneEbay.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace CloneEbay.Pages
{
    public class PaymentModel : PageModel
    {
        private readonly CloneEbayDbContext _db;
        public PaymentModel(CloneEbayDbContext db)
        {
            _db = db;
        }

        public OrderTable? Order { get; set; }

        public async Task<IActionResult> OnGetAsync(int orderId)
        {
            Order = await _db.OrderTables
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .Include(o => o.Address)
                .FirstOrDefaultAsync(o => o.Id == orderId);
            if (Order == null)
                return RedirectToPage("/Error");
            return Page();
        }

        public async Task<IActionResult> OnPostPayAsync(int orderId)
        {
            var order = await _db.OrderTables.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
                return RedirectToPage("/Error");
            order.Status = "Paid";
            await _db.SaveChangesAsync();
            
            // Redirect to order confirmation page
            return RedirectToPage("/OrderConfirmation", new { orderId = order.Id });
        }
    }
} 