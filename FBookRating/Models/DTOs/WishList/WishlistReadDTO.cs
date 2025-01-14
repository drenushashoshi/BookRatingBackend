namespace FBookRating.Models.DTOs.WishList
{
    public class WishlistReadDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public IEnumerable<WishlistBookDTO> Books { get; set; }
    }
}
