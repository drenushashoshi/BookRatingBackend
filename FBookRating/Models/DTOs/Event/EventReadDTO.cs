namespace FBookRating.Models.DTOs.Event
{
    public class EventReadDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public string Description { get; set; }
        public IEnumerable<BookEventDTO> Books { get; set; }
    }
}
