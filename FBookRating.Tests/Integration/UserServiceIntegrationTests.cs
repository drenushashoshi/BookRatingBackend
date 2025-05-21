using Xunit;
using Data_Access_Layer.Entities;
using Data_Access_Layer.UnitOfWork;
using FBookRating.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using Data_Access_Layer;

namespace FBookRating.Tests.Integration
{
    public class UserServiceIntegrationTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _service;
        private readonly IUnitOfWork _unitOfWork;

        public UserServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "UserServiceIntegrationTests")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
            _unitOfWork = new UnitOfWork(_context);
            _service = new UserService(_unitOfWork);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task UserService_CreateOrUpdateUser_ShouldCreateNewUser()
        {
            // Arrange
            var userId = "auth0|test-user-id";
            var userName = "testuser";
            var displayName = "Test User";
            var email = "test@example.com";
            var profilePictureUrl = "https://example.com/profile.jpg";

            // Act
            await _service.CreateOrUpdateUserAsync(userId, userName, displayName, email, profilePictureUrl);

            // Assert
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            Assert.NotNull(user);
            Assert.Equal(userName, user.UserName);
            Assert.Equal(displayName, user.DisplayName);
            Assert.Equal(email, user.Email);
            Assert.Equal(profilePictureUrl, user.ProfilePictureUrl);
        }

        [Fact]
        public async Task UserService_CreateOrUpdateUser_ShouldUpdateExistingUser()
        {
            // Arrange
            var userId = "auth0|test-user-id";
            var existingUser = new ApplicationUser
            {
                Id = userId,
                UserName = "oldusername",
                DisplayName = "Old Name",
                Email = "old@example.com",
                ProfilePictureUrl = "https://example.com/old.jpg"
            };
            _context.Users.Add(existingUser);
            await _context.SaveChangesAsync();

            // Act
            var newUserName = "newusername";
            var newDisplayName = "New Name";
            var newEmail = "new@example.com";
            var newProfilePictureUrl = "https://example.com/new.jpg";
            await _service.CreateOrUpdateUserAsync(userId, newUserName, newDisplayName, newEmail, newProfilePictureUrl);

            // Assert
            var updatedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            Assert.NotNull(updatedUser);
            Assert.Equal(newUserName, updatedUser.UserName);
            Assert.Equal(newDisplayName, updatedUser.DisplayName);
            Assert.Equal(newEmail, updatedUser.Email);
            Assert.Equal(newProfilePictureUrl, updatedUser.ProfilePictureUrl);
        }

        [Fact]
        public async Task UserService_CreateOrUpdateUser_ShouldHandleEmptyUsername()
        {
            // Arrange
            var userId = "auth0|social-user-id";
            var userName = string.Empty; // Social login might not provide username
            var displayName = "Social User";
            var email = "social@example.com";
            var profilePictureUrl = "https://example.com/profile.jpg";

            // Act
            await _service.CreateOrUpdateUserAsync(userId, userName, displayName, email, profilePictureUrl);

            // Assert
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            Assert.NotNull(user);
            Assert.Equal(string.Empty, user.UserName); // Service leaves username empty
            Assert.Equal(displayName, user.DisplayName);
            Assert.Equal(email, user.Email);
            Assert.Equal(profilePictureUrl, user.ProfilePictureUrl);
        }
    }
} 