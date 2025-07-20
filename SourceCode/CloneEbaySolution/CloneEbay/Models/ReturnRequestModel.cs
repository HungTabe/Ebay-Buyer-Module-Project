using System.ComponentModel.DataAnnotations;

namespace CloneEbay.Models
{
    public class CreateReturnRequestModel
    {
        [Required(ErrorMessage = "Order ID is required")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Reason is required")]
        [StringLength(1000, ErrorMessage = "Reason cannot exceed 1000 characters")]
        public string Reason { get; set; } = "";
    }

    public class ReturnRequestViewModel
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string Reason { get; set; } = "";
        public string Status { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public OrderSummaryViewModel? Order { get; set; }
    }

    public class ReturnRequestListViewModel
    {
        public List<ReturnRequestViewModel> ReturnRequests { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
} 