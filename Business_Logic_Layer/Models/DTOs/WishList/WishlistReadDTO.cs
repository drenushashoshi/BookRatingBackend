using System.ComponentModel.DataAnnotations;

namespace FBookRating.Models.DTOs.WishList
{
    public class WishlistReadDTO
    {
        [Required(ErrorMessage = "Wishlist ID is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Wishlist name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; }

        public IEnumerable<WishlistBookDTO> Books { get; set; }
    }
}
