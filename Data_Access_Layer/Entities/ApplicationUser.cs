

namespace Data_Access_Layer.Entities
{
    public class ApplicationUser
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // Composition: A user "owns" their wishlists.
        public ICollection<Wishlist> Wishlists { get; set; }

        // Association: A user can write reviews/ratings for multiple books.
        public ICollection<ReviewRating> ReviewRatings { get; set; }
    }
}
