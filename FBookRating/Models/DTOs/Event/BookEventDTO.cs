namespace FBookRating.Models.DTOs.Event
{
    public class BookEventDTO
    {
        public int EventId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
    }
}
