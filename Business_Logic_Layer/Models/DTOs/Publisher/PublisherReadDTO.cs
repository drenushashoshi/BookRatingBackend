using System.ComponentModel.DataAnnotations;

namespace FBookRating.Models.DTOs.Publisher
{
    public class PublisherReadDTO
    {
        [Required(ErrorMessage = "Publisher ID is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Publisher name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(200, ErrorMessage = "Website URL cannot exceed 200 characters")]
        public string Website { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }
    }
}
