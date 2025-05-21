using Xunit;
using Data_Access_Layer.Entities;
using Data_Access_Layer.UnitOfWork;
using FBookRating.Services;
using FBookRating.Models.DTOs.WishList;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using Data_Access_Layer;
using System.ComponentModel.DataAnnotations;

namespace FBookRating.Tests.Integration
{
    public class WishlistServiceIntegrationTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly WishlistService _service;
        private readonly IUnitOfWork _unitOfWork;

        public WishlistServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "WishlistServiceIntegrationTests")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
            _unitOfWork = new UnitOfWork(_context);
            _service = new WishlistService(_unitOfWork);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task WishlistService_CompleteFlow_ShouldWork()
        {
            // Arrange
            var userId = "test-user-id";
            var createDto = new WishlistCreateDTO { Name = "Test Wishlist" };
            var bookId = Guid.NewGuid();

            // Create a book first
            _context.Books.Add(new Book
            {
                Id = bookId,
                Title = "Test Book",
                CoverImageUrl = "cover.jpg",
                Description = "Test Description",
                ISBN = "1234567890",
                PublishedDate = DateTime.UtcNow,
                CategoryId = Guid.NewGuid()
            });
            await _context.SaveChangesAsync();

            // Act - Create Wishlist
            await _service.AddWishlistAsync(createDto, userId);
            var createdWishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Name == createDto.Name);

            // Assert - Create
            Assert.NotNull(createdWishlist);
            Assert.Equal(createDto.Name, createdWishlist.Name);
            Assert.Equal(userId, createdWishlist.UserId);

            // Act - Add Book
            await _service.AddBookToWishlistAsync(createdWishlist.Id, bookId);

            // Assert - Add Book
            var wishlistBooks = await _context.WishlistBooks
                .Where(wb => wb.WishlistId == createdWishlist.Id)
                .ToListAsync();
            Assert.Single(wishlistBooks);
            Assert.Equal(bookId, wishlistBooks[0].BookId);

            // Act - Remove Book
            await _service.RemoveBookFromWishlistAsync(createdWishlist.Id, bookId);

            // Assert - Remove Book
            wishlistBooks = await _context.WishlistBooks
                .Where(wb => wb.WishlistId == createdWishlist.Id)
                .ToListAsync();
            Assert.Empty(wishlistBooks);
        }

        [Fact]
        public async Task WishlistService_WithInvalidData_ShouldThrowValidationException()
        {
            // Arrange
            var userId = "test-user-id";
            var invalidDto = new WishlistCreateDTO { Name = "A" }; // Too short

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.AddWishlistAsync(invalidDto, userId));
        }

        [Fact]
        public async Task WishlistService_AddDuplicateBook_ShouldThrowValidationException()
        {
            // Arrange
            var userId = "test-user-id";
            var wishlistDto = new WishlistCreateDTO { Name = "Test Wishlist" };
            var bookId = Guid.NewGuid();

            // Create a book
            _context.Books.Add(new Book
            {
                Id = bookId,
                Title = "Test Book",
                CoverImageUrl = "cover.jpg",
                Description = "Test Description",
                ISBN = "1234567890",
                PublishedDate = DateTime.UtcNow,
                CategoryId = Guid.NewGuid()
            });
            await _context.SaveChangesAsync();

            // Create wishlist and add book
            await _service.AddWishlistAsync(wishlistDto, userId);
            var wishlist = await _context.Wishlists.FirstOrDefaultAsync(w => w.Name == wishlistDto.Name);
            Assert.NotNull(wishlist);
            await _service.AddBookToWishlistAsync(wishlist.Id, bookId);

            // Act & Assert - Try to add the same book again
            await Assert.ThrowsAsync<ValidationException>(() => 
                _service.AddBookToWishlistAsync(wishlist.Id, bookId));
        }

        [Fact]
        public async Task WishlistService_GetUserWishlists_ShouldReturnCorrectWishlists()
        {
            // Arrange
            var userId = "test-user-id";
            var otherUserId = "other-user-id";

            // Create wishlists for different users
            await _service.AddWishlistAsync(new WishlistCreateDTO { Name = "User Wishlist" }, userId);
            await _service.AddWishlistAsync(new WishlistCreateDTO { Name = "Other User Wishlist" }, otherUserId);

            // Act
            var userWishlists = await _service.GetWishlistsByUserAsync(userId);

            // Assert
            Assert.Single(userWishlists);
            var firstWishlist = userWishlists.First();
            Assert.Equal("User Wishlist", firstWishlist.Name);
            Assert.Equal(userId, firstWishlist.UserId);
        }
    }
} 