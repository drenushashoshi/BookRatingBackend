using Xunit;
using Data_Access_Layer.Entities;
using Data_Access_Layer.UnitOfWork;
using FBookRating.Services;
using FBookRating.Models.DTOs.WishList;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Data_Access_Layer;

namespace FBookRating.Tests.Services
{
    public class WishlistServiceTests
    {
        private DbContextOptions<ApplicationDbContext> CreateNewContextOptions(string dbName)
            => new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

        [Fact]
        public async Task GetWishlistsByUserAsync_ShouldReturnUserWishlists()
        {
            var opts = CreateNewContextOptions(nameof(GetWishlistsByUserAsync_ShouldReturnUserWishlists));
            var userId = "test-user-id";
            var wishlistId = Guid.NewGuid();
            using (var seedContext = new ApplicationDbContext(opts))
            {
                var bookId = Guid.NewGuid();
                seedContext.Books.Add(new Book {
                    Id = bookId,
                    Title = "Book 1",
                    CoverImageUrl = "cover1.jpg",
                    Description = "desc1",
                    ISBN = "isbn1",
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = Guid.NewGuid()
                });
                seedContext.Wishlists.Add(new Wishlist
                {
                    Id = wishlistId,
                    Name = "Wishlist 1",
                    UserId = userId,
                    WishlistBooks = new List<WishlistBook>
                    {
                        new WishlistBook
                        {
                            BookId = bookId,
                            AddedDate = DateTime.UtcNow
                        }
                    }
                });
                seedContext.SaveChanges();
            }

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new WishlistService(new UnitOfWork(context));
                var result = await service.GetWishlistsByUserAsync(userId);
                Assert.NotNull(result);
                Assert.Single(result);
                Assert.Equal("Wishlist 1", result.First().Name);
                Assert.Single(result.First().Books);
            }
        }

        [Fact]
        public async Task GetWishlistsByUserAsync_WhenUserHasNoWishlists_ShouldReturnEmptyList()
        {
            var opts = CreateNewContextOptions(nameof(GetWishlistsByUserAsync_WhenUserHasNoWishlists_ShouldReturnEmptyList));
            var userId = "test-user-id";
            // No seeding
            using (var context = new ApplicationDbContext(opts))
            {
                var service = new WishlistService(new UnitOfWork(context));
                var result = await service.GetWishlistsByUserAsync(userId);
                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }

        [Fact]
        public async Task AddWishlistAsync_ShouldCreateNewWishlist()
        {
            var opts = CreateNewContextOptions(nameof(AddWishlistAsync_ShouldCreateNewWishlist));
            var userId = "test-user-id";
            var wishlistDTO = new WishlistCreateDTO { Name = "New Wishlist" };

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new WishlistService(new UnitOfWork(context));
                await service.AddWishlistAsync(wishlistDTO, userId);
            }

            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var wishlist = verifyContext.Wishlists.SingleOrDefault(w => w.UserId == userId);
                Assert.NotNull(wishlist);
                Assert.Equal("New Wishlist", wishlist.Name);
            }
        }

        [Fact]
        public async Task AddBookToWishlistAsync_WhenBookNotInWishlist_ShouldAddBook()
        {
            var opts = CreateNewContextOptions(nameof(AddBookToWishlistAsync_WhenBookNotInWishlist_ShouldAddBook));
            var wishlistId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Books.Add(new Book {
                    Id = bookId,
                    Title = "Book",
                    CoverImageUrl = "cover.jpg",
                    Description = "desc",
                    ISBN = "isbn",
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = Guid.NewGuid()
                });
                seedContext.Wishlists.Add(new Wishlist { Id = wishlistId, Name = "Wishlist", UserId = "user" });
                seedContext.SaveChanges();
            }

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new WishlistService(new UnitOfWork(context));
                await service.AddBookToWishlistAsync(wishlistId, bookId);
            }

            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var wishlistBook = verifyContext.WishlistBooks.SingleOrDefault(wb => wb.WishlistId == wishlistId && wb.BookId == bookId);
                Assert.NotNull(wishlistBook);
            }
        }

        [Fact]
        public async Task AddBookToWishlistAsync_WhenBookAlreadyInWishlist_ShouldNotAddBook()
        {
            var opts = CreateNewContextOptions(nameof(AddBookToWishlistAsync_WhenBookAlreadyInWishlist_ShouldNotAddBook));
            var wishlistId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Books.Add(new Book {
                    Id = bookId,
                    Title = "Book",
                    CoverImageUrl = "cover.jpg",
                    Description = "desc",
                    ISBN = "isbn",
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = Guid.NewGuid()
                });
                seedContext.Wishlists.Add(new Wishlist { Id = wishlistId, Name = "Wishlist", UserId = "user" });
                seedContext.WishlistBooks.Add(new WishlistBook { WishlistId = wishlistId, BookId = bookId });
                seedContext.SaveChanges();
            }

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new WishlistService(new UnitOfWork(context));
                await service.AddBookToWishlistAsync(wishlistId, bookId);
            }

            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var count = verifyContext.WishlistBooks.Count(wb => wb.WishlistId == wishlistId && wb.BookId == bookId);
                Assert.Equal(1, count); // Should still be only one
            }
        }

        [Fact]
        public async Task RemoveBookFromWishlistAsync_WhenBookExists_ShouldRemoveBook()
        {
            var opts = CreateNewContextOptions(nameof(RemoveBookFromWishlistAsync_WhenBookExists_ShouldRemoveBook));
            var wishlistId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Books.Add(new Book {
                    Id = bookId,
                    Title = "Book",
                    CoverImageUrl = "cover.jpg",
                    Description = "desc",
                    ISBN = "isbn",
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = Guid.NewGuid()
                });
                seedContext.Wishlists.Add(new Wishlist { Id = wishlistId, Name = "Wishlist", UserId = "user" });
                seedContext.WishlistBooks.Add(new WishlistBook { WishlistId = wishlistId, BookId = bookId });
                seedContext.SaveChanges();
            }

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new WishlistService(new UnitOfWork(context));
                await service.RemoveBookFromWishlistAsync(wishlistId, bookId);
            }

            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var wishlistBook = verifyContext.WishlistBooks.SingleOrDefault(wb => wb.WishlistId == wishlistId && wb.BookId == bookId);
                Assert.Null(wishlistBook);
            }
        }

        [Fact]
        public async Task RemoveBookFromWishlistAsync_WhenBookDoesNotExist_ShouldNotThrowException()
        {
            var opts = CreateNewContextOptions(nameof(RemoveBookFromWishlistAsync_WhenBookDoesNotExist_ShouldNotThrowException));
            var wishlistId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            // No seeding
            using (var context = new ApplicationDbContext(opts))
            {
                var service = new WishlistService(new UnitOfWork(context));
                await service.RemoveBookFromWishlistAsync(wishlistId, bookId);
            }
            // No exception means pass
        }
    }
} 