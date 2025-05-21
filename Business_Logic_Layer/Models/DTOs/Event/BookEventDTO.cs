using System.ComponentModel.DataAnnotations;

namespace FBookRating.Models.DTOs.Event
{
    public class BookEventDTO
    {
        [Required(ErrorMessage = "Event ID is required")]
        public Guid EventId { get; set; }

        [Required(ErrorMessage = "Book ID is required")]
        public Guid BookId { get; set; }

        [Required(ErrorMessage = "Book title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Book title must be between 1 and 200 characters")]
        public string BookTitle { get; set; }
    }
}
