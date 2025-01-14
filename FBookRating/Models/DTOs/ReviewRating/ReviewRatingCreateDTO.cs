namespace FBookRating.Models.DTOs.ReviewRating
{
    public class ReviewRatingCreateDTO
    {
        public int Score { get; set; } // Rating (1-5)
        public string ReviewText { get; set; }
        public int BookId { get; set; }
    }
}
