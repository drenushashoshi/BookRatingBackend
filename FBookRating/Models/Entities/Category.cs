namespace FBookRating.Models.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        // Association: A category can group multiple books.
        public ICollection<Book> Books { get; set; }
    }

}
