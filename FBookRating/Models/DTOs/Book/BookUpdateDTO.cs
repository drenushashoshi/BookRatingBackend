namespace FBookRating.Models.DTOs.Book
{
    public class BookUpdateDTO
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public string CoverImageUrl { get; set; }
        public int CategoryId { get; set; }
        public int? AuthorId { get; set; }
        public int? PublisherId { get; set; }
    }
}
