using System.ComponentModel.DataAnnotations;

namespace FBookRating.Models.DTOs.WishList
{
    public class WishlistCreateDTO
    {
        [Required(ErrorMessage = "Wishlist name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 50 characters")]
        public string Name { get; set; }
    }
}
