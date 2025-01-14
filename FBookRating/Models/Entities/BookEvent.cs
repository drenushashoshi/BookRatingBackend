namespace FBookRating.Models.Entities
{
    public class BookEvent
    {
        public int EventId { get; set; }
        public Event Event { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
    }

}
