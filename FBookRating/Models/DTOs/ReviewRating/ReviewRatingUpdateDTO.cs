namespace FBookRating.Models.DTOs.ReviewRating
{
    public class ReviewRatingUpdateDTO
    {
        public int Score { get; set; } // Rating (1-5)
        public string ReviewText { get; set; }
    }
}
