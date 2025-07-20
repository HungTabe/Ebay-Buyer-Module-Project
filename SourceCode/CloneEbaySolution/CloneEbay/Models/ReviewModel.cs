namespace CloneEbay.Models
{
    public class ReviewModel
    {
        public int ProductId { get; set; }
        public int Rating { get; set; } // Star rating (1-5)
        public string Comment { get; set; } = string.Empty;
    }

    public class ReviewViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ReviewerId { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class ProductReviewViewModel
    {
        public int ProductId { get; set; }
        public string ProductTitle { get; set; } = string.Empty;
        public List<ReviewViewModel> Reviews { get; set; } = new();
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public ReviewModel NewReview { get; set; } = new();
    }
} 