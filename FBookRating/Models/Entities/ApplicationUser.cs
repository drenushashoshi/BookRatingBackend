using Microsoft.AspNetCore.Identity;

namespace FBookRating.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string DisplayName { get; set; }
        public string ProfilePictureUrl { get; set; }

        // Composition: A user "owns" their wishlists.
        public ICollection<Wishlist> Wishlists { get; set; }

        // Association: A user can write reviews/ratings for multiple books.
        public ICollection<ReviewRating> ReviewRatings { get; set; }
    }
}
