namespace Data_Access_Layer.Entities
{
    public class Wishlist : BaseEntity
    {
        public string Name { get; set; }

        // Composition: A wishlist belongs to a specific user.
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        // Association: A wishlist contains books.
        public ICollection<WishlistBook> WishlistBooks { get; set; }
    }

}
