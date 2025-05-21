using System.ComponentModel.DataAnnotations;

namespace FBookRating.Models.DTOs.WishList
{
    public class WishlistBookDTO
    {
        [Required(ErrorMessage = "Wishlist ID is required")]
        public Guid WishlistId { get; set; }

        [Required(ErrorMessage = "Book ID is required")]
        public Guid BookId { get; set; }

        [Required(ErrorMessage = "Book title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Book title must be between 1 and 200 characters")]
        public string BookTitle { get; set; }

        [Required(ErrorMessage = "Added date is required")]
        [DataType(DataType.DateTime)]
        public DateTime AddedDate { get; set; }
    }
}
