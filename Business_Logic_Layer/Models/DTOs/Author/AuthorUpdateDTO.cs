using System.ComponentModel.DataAnnotations;

namespace FBookRating.Models.DTOs.Author
{
    public class AuthorUpdateDTO
    {
        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name should be between 2 and 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Biography field is required.")]
        [StringLength(1000, ErrorMessage = "Biography cannot exceed 1000 characters.")]
        public string Biography { get; set; }

        [Required(ErrorMessage = "The BirthDate field is required.")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}
