using Microsoft.AspNetCore.Http;

namespace FBookRating.Models.DTOs.Book
{
    public class BookUpdateDTO
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public IFormFile CoverImage { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? AuthorId { get; set; }
        public Guid? PublisherId { get; set; }
    }
}
