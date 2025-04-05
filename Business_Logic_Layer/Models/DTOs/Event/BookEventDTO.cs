namespace FBookRating.Models.DTOs.Event
{
    public class BookEventDTO
    {
        public Guid EventId { get; set; }
        public Guid BookId { get; set; }
        public string BookTitle { get; set; }
    }
}
