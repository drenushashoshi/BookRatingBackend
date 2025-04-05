namespace Data_Access_Layer.Entities
{
    public class Publisher : BaseEntity
    {
        public string Name { get; set; }
        public string Website { get; set; }
        public string Address { get; set; }

        // Aggregation: A publisher releases multiple books.
        public ICollection<Book> Books { get; set; }
    }

}
