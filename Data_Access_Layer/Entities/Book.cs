using Azure;

namespace Data_Access_Layer.Entities
{
    public class Book : BaseEntity
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public DateTime PublishedDate { get; set; }
        public string CoverImageUrl { get; set; }

        // Association: A book is reviewed and rated by users.
        public ICollection<ReviewRating> ReviewRatings { get; set; }

        // Association: A book belongs to one category
        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        // Association: A book can have multiple tags.
        public ICollection<Tag> Tags { get; set; }


        // Aggregation
        public Guid? AuthorId { get; set; }
        public Author Author { get; set; }

        // Aggregation
        public Guid? PublisherId { get; set; }
        public Publisher Publisher { get; set; }



        public ICollection<WishlistBook> WishlistBooks { get; set; }

    }

}
