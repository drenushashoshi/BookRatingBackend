namespace FBookRating.Models.DTOs.WishList
{
    public class WishlistBookDTO
    {
        public int WishlistId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
