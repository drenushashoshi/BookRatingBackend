namespace FBookRating.Models.DTOs.Event
{
    public class EventCreateDTO
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public string Description { get; set; }
    }
}
