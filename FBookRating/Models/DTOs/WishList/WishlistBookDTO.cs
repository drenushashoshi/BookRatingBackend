namespace FBookRating.Models.DTOs.WishList
{
    public class WishlistBookDTO
    {
        public Guid WishlistId { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
