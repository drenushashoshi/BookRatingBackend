namespace FBookRating.Models.DTOs.Book
{
    public class BookReadDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public string CoverImageUrl { get; set; }
        public string AuthorName { get; set; } // For displaying the author's name
        public string PublisherName { get; set; } // For displaying the publisher's name
        public string CategoryName { get; set; } // For displaying the category's name
    }
}
