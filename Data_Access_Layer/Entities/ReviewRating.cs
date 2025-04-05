namespace Data_Access_Layer.Entities
{
    public class ReviewRating : BaseEntity
    {
        public int Score { get; set; } // Rating (1-5)
        public string ReviewText { get; set; }

        // Association: Links a user to a book.
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public Guid BookId { get; set; }
        public Book Book { get; set; }
    }

}
