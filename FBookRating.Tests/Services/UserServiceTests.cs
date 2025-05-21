using Xunit;
using Data_Access_Layer.Entities;
using Data_Access_Layer.UnitOfWork;
using FBookRating.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using Data_Access_Layer;

namespace FBookRating.Tests.Services
{
    public class UserServiceTests
    {
        private DbContextOptions<ApplicationDbContext> CreateNewContextOptions(string dbName)
            => new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

        [Fact]
        public async Task CreateOrUpdateUserAsync_WithNewUser_ShouldCreateUser()
        {
            var opts = CreateNewContextOptions(nameof(CreateOrUpdateUserAsync_WithNewUser_ShouldCreateUser));
            var userId = "auth0|test-user-id";
            var userName = "testuser";
            var displayName = "Test User";
            var email = "test@example.com";
            var profilePictureUrl = "https://example.com/profile.jpg";

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new UserService(new UnitOfWork(context));
                await service.CreateOrUpdateUserAsync(userId, userName, displayName, email, profilePictureUrl);
            }

            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var user = verifyContext.Users.SingleOrDefault(u => u.Id == userId);
                Assert.NotNull(user);
                Assert.Equal(userName, user.UserName);
                Assert.Equal(displayName, user.DisplayName);
                Assert.Equal(email, user.Email);
                Assert.Equal(profilePictureUrl, user.ProfilePictureUrl);
            }
        }

        [Fact]
        public async Task CreateOrUpdateUserAsync_WithExistingUser_ShouldUpdateUser()
        {
            var opts = CreateNewContextOptions(nameof(CreateOrUpdateUserAsync_WithExistingUser_ShouldUpdateUser));
            var userId = "auth0|test-user-id";
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Users.Add(new ApplicationUser
                {
                    Id = userId,
                    UserName = "oldusername",
                    DisplayName = "Old Name",
                    Email = "old@example.com",
                    ProfilePictureUrl = "https://example.com/old.jpg"
                });
                seedContext.SaveChanges();
            }

            var userName = "testuser";
            var displayName = "Test User";
            var email = "test@example.com";
            var profilePictureUrl = "https://example.com/profile.jpg";

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new UserService(new UnitOfWork(context));
                await service.CreateOrUpdateUserAsync(userId, userName, displayName, email, profilePictureUrl);
            }

            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var user = verifyContext.Users.SingleOrDefault(u => u.Id == userId);
                Assert.NotNull(user);
                Assert.Equal(userName, user.UserName);
                Assert.Equal(displayName, user.DisplayName);
                Assert.Equal(email, user.Email);
                Assert.Equal(profilePictureUrl, user.ProfilePictureUrl);
            }
        }

        [Fact]
        public async Task CreateOrUpdateUserAsync_WithInvalidAuth0Id_ShouldNotThrowException()
        {
            var opts = CreateNewContextOptions(nameof(CreateOrUpdateUserAsync_WithInvalidAuth0Id_ShouldNotThrowException));
            var invalidUserId = "invalid-user-id"; // Missing Auth0 prefix
            var userName = "testuser";
            var displayName = "Test User";
            var email = "test@example.com";
            var profilePictureUrl = "https://example.com/profile.jpg";

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new UserService(new UnitOfWork(context));
                await service.CreateOrUpdateUserAsync(invalidUserId, userName, displayName, email, profilePictureUrl);
                // No exception expected
            }
        }

        [Fact]
        public async Task CreateOrUpdateUserAsync_WithSocialLogin_ShouldHandleEmptyUsername()
        {
            var opts = CreateNewContextOptions(nameof(CreateOrUpdateUserAsync_WithSocialLogin_ShouldHandleEmptyUsername));
            var userId = "auth0|social-user-id";
            var userName = ""; // Social login might not provide username
            var displayName = "Social User";
            var email = "social@example.com";
            var profilePictureUrl = "https://example.com/profile.jpg";

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new UserService(new UnitOfWork(context));
                await service.CreateOrUpdateUserAsync(userId, userName, displayName, email, profilePictureUrl);
            }

            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var user = verifyContext.Users.SingleOrDefault(u => u.Id == userId);
                Assert.NotNull(user);
                Assert.Equal("", user.UserName); // Service leaves username empty
                Assert.Equal(displayName, user.DisplayName);
                Assert.Equal(email, user.Email);
                Assert.Equal(profilePictureUrl, user.ProfilePictureUrl);
            }
        }
    }
} 