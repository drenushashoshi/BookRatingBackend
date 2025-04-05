namespace Data_Access_Layer.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }

        // Association: Tags apply to books.
        public ICollection<Book> Books { get; set; }
    }

}
