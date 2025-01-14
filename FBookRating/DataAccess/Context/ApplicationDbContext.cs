using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FBookRating.Models.Entities;

namespace FBookRating.DataAccess.Context
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        base(options)
        { }



        // DbSets for application-specific entities
        public DbSet<Book> Books { get; set; }
        public DbSet<ReviewRating> ReviewRatings { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<BookEvent> BookEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // -------------------------------------------
            // One-to-Many: Category ↔ Book (Association)
            // A book belongs to one category, and a category can group multiple books.
            // This is an Association because neither entity "owns" the other. 
            // Both books and categories can exist independently.
            // -------------------------------------------
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of category if books exist.

            // -------------------------------------------
            // One-to-Many: Publisher ↔ Book (Aggregation)
            // A publisher releases multiple books, but books can exist independently of a publisher.
            // This is Aggregation because the publisher loosely "owns" the books.
            // Deleting a publisher nullifies the PublisherId in the Book table.
            // -------------------------------------------
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PublisherId)
                .OnDelete(DeleteBehavior.SetNull); // Nullify PublisherId if publisher is deleted.

            // -------------------------------------------
            // One-to-Many: Author ↔ Book (Aggregation)
            // An author writes multiple books, but books can exist independently of an author.
            // This is Aggregation because the author loosely "owns" the books.
            // Deleting an author nullifies the AuthorId in the Book table.
            // -------------------------------------------
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.SetNull); // Nullify AuthorId if the author is deleted.

            // -------------------------------------------
            // One-to-Many: ReviewRating ↔ Book (Association)
            // A book is reviewed by multiple users, but the reviews are not owned by the book.
            // This is an Association because the reviews are a record of interaction and not part of the book's lifecycle.
            // -------------------------------------------
            modelBuilder.Entity<ReviewRating>()
                .HasOne(r => r.Book)
                .WithMany(b => b.ReviewRatings)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of book if reviews exist.

            // -------------------------------------------
            // One-to-Many: ReviewRating ↔ ApplicationUser (Association)
            // A user writes multiple reviews, but the reviews are not owned by the user.
            // This is an Association because reviews are independent records of interaction between users and books.
            // -------------------------------------------
            modelBuilder.Entity<ReviewRating>()
                .HasOne(r => r.User)
                .WithMany(u => u.ReviewRatings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of user if reviews exist.

            // -------------------------------------------
            // One-to-Many: Wishlist ↔ ApplicationUser (Composition)
            // A user owns their wishlists, and deleting the user deletes their wishlists.
            // This is Composition because the lifecycle of the wishlist is strictly tied to the user.
            // -------------------------------------------
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishlists)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete ensures wishlists are deleted with the user.

            // -------------------------------------------
            // Many-to-Many: Wishlist ↔ Book via WishlistBook (Explicit Join Entity)
            // A wishlist contains multiple books, and books can appear in multiple wishlists.
            // This is Composition because the wishlist owns the relationship with books.
            // -------------------------------------------
            modelBuilder.Entity<WishlistBook>()
                .HasKey(wb => new { wb.WishlistId, wb.BookId }); // Composite primary key

            modelBuilder.Entity<WishlistBook>()
                .HasOne(wb => wb.Wishlist)
                .WithMany(w => w.WishlistBooks)
                .HasForeignKey(wb => wb.WishlistId)
                .OnDelete(DeleteBehavior.Cascade); // Deleting the wishlist removes related WishlistBooks.

            modelBuilder.Entity<WishlistBook>()
                .HasOne(wb => wb.Book)
                .WithMany(b => b.WishlistBooks)
                .HasForeignKey(wb => wb.BookId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of books if they are part of wishlists.




            modelBuilder.Entity<BookEvent>()
            .HasKey(be => new { be.EventId, be.BookId }); // Composite primary key

            // -------------------------------------------
            // One-to-Many: Event ↔ BookEvent (Composition)
            // An event owns its book-event mappings, and deleting the event deletes all mappings.
            // This is Composition because the book-event mapping cannot exist without the event.
            // -------------------------------------------
            modelBuilder.Entity<BookEvent>()
                .HasOne(be => be.Event)
                .WithMany(e => e.BookEvents)
                .HasForeignKey(be => be.EventId)
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete ensures book-event mappings are deleted with the event.

            // -------------------------------------------
            // Many-to-One: BookEvent ↔ Book (Association)
            // A book can be part of multiple events, but deleting a book does not affect the event.
            // This is an Association because events and books are loosely coupled.
            // -------------------------------------------
            modelBuilder.Entity<BookEvent>()
                .HasOne(be => be.Book)
                .WithMany()
                .HasForeignKey(be => be.BookId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of book if it is part of events.
        }




    }
}
