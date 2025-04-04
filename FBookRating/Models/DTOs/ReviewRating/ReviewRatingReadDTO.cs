namespace FBookRating.Models.DTOs.ReviewRating
{
    public class ReviewRatingReadDTO
    {
        public Guid Id { get; set; }
        public int Score { get; set; }
        public string ReviewText { get; set; }
        public string UserName { get; set; } // Display the user's name
        public string BookTitle { get; set; } // Display the book title
    }
}
