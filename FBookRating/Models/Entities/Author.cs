namespace FBookRating.Models.Entities
{
    public class Author : BaseEntity
    {
        public string Name { get; set; }
        public string Biography { get; set; }
        public DateTime BirthDate { get; set; }

        // Aggregation: An author writes multiple books.
        public ICollection<Book> Books { get; set; }
    }

}
