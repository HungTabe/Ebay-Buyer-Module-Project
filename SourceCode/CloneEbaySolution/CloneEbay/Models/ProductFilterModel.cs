namespace CloneEbay.Models
{
    public class ProductFilterModel
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; } // "name", "price", "date"
        public string? SortOrder { get; set; } // "asc", "desc"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public bool IsAuction { get; set; }
        public string? Condition { get; set; }
    }

    public class ProductListViewModel
    {
        public List<ProductViewModel> Products { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public ProductFilterModel Filter { get; set; } = new();
        public List<CategoryViewModel> Categories { get; set; } = new();
    }

    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Images { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public bool IsAuction { get; set; }
        public DateTime? AuctionEndTime { get; set; }
        public string Condition { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int BidCount { get; set; }
        public decimal? CurrentBid { get; set; }
    }

    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public int ProductCount { get; set; }
    }
} 