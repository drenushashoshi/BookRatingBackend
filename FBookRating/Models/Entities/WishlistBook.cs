namespace FBookRating.Models.Entities
{
    public class WishlistBook
    {
        public Guid WishlistId { get; set; }
        public Wishlist Wishlist { get; set; }

        public Guid BookId { get; set; }
        public Book Book { get; set; }

        // Additional properties for the join table
        public DateTime AddedDate { get; set; } // Tracks when the book was added to the wishlist
    }

}
