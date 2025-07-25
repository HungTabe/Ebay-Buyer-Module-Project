using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloneEbay.Data;
using CloneEbay.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace CloneEbay.Pages
{
    public class ShippingManagementModel : PageModel
    {
        private readonly CloneEbayDbContext _db;
        public ShippingManagementModel(CloneEbayDbContext db)
        {
            _db = db;
        }

        public List<ShippingInfo> Shipments { get; set; } = new();
        public string? LogContent { get; set; }

        [BindProperty]
        public int UpdateShipmentId { get; set; }
        [BindProperty]
        public string UpdateStatus { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            Shipments = await _db.ShippingInfos
                .Include(s => s.Order)
                    .ThenInclude(o => o.Buyer)
                .Include(s => s.Order)
                    .ThenInclude(o => o.Address)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync()
        {
            var shipment = await _db.ShippingInfos.Include(s => s.Order).FirstOrDefaultAsync(s => s.Id == UpdateShipmentId);
            if (shipment != null && !string.IsNullOrEmpty(UpdateStatus))
            {
                shipment.Status = UpdateStatus;
                await _db.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public void OnGetViewLog()
        {
            var logPath = Path.Combine("wwwroot", "logs", "transaction.log");
            if (System.IO.File.Exists(logPath))
            {
                LogContent = System.IO.File.ReadAllText(logPath);
            }
            else
            {
                LogContent = "No log found.";
            }
            Shipments = _db.ShippingInfos
                .Include(s => s.Order)
                    .ThenInclude(o => o.Buyer)
                .Include(s => s.Order)
                    .ThenInclude(o => o.Address)
                .ToList();
        }
    }
} 