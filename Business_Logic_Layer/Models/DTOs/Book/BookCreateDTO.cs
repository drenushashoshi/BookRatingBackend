using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FBookRating.Models.DTOs.Book
{
    public class BookCreateDTO
    {
        [Required(ErrorMessage = "The Title field is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title should be between 2 and 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "The ISBN field is required.")]
        [RegularExpression(@"^(97(8|9))?\d{9}(\d|X)$", ErrorMessage = "Please enter a valid ISBN number.")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "The Description field is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The Published Date field is required.")]
        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; }

        [Required(ErrorMessage = "The Cover Image is required.")]
        public IFormFile CoverImage { get; set; }

        [Required(ErrorMessage = "The Category field is required.")]
        public Guid CategoryId { get; set; }

        public Guid? AuthorId { get; set; }
        public Guid? PublisherId { get; set; }
    }
}
