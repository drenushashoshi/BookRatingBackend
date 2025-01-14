namespace FBookRating.Models.Entities
{
    public class Event : BaseEntity
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime StartDate { get; set; }
        public string Description { get; set; }

        // Composition: An event owns its featured books.
        public ICollection<BookEvent> BookEvents { get; set; }
    }

}
