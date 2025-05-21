using Xunit;
using Data_Access_Layer.Entities;
using Data_Access_Layer.UnitOfWork;
using FBookRating.Services;
using FBookRating.Models.DTOs.Publisher;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using Data_Access_Layer;
using System.ComponentModel.DataAnnotations;

namespace FBookRating.Tests.Services
{
    public class PublisherServiceTests
    {
        private DbContextOptions<ApplicationDbContext> CreateNewContextOptions(string dbName)
            => new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

        [Fact]
        public async Task GetAllPublishersAsync_ShouldReturnAllPublishers()
        {
            var opts = CreateNewContextOptions(nameof(GetAllPublishersAsync_ShouldReturnAllPublishers));
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Publishers.AddRange(
                    new Publisher { Id = Guid.NewGuid(), Name = "Publisher 1", Website = "www.publisher1.com", Address = "Address 1" },
                    new Publisher { Id = Guid.NewGuid(), Name = "Publisher 2", Website = "www.publisher2.com", Address = "Address 2" }
                );
                seedContext.SaveChanges();
            }

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new PublisherService(new UnitOfWork(context));
                var result = await service.GetAllPublishersAsync();
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetPublisherByIdAsync_WithValidId_ShouldReturnPublisher()
        {
            var opts = CreateNewContextOptions(nameof(GetPublisherByIdAsync_WithValidId_ShouldReturnPublisher));
            var publisherId = Guid.NewGuid();
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Publishers.Add(new Publisher
                {
                    Id = publisherId,
                    Name = "Test Publisher",
                    Website = "www.test.com",
                    Address = "Test Address"
                });
                seedContext.SaveChanges();
            }

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new PublisherService(new UnitOfWork(context));
                var result = await service.GetPublisherByIdAsync(publisherId);
                Assert.NotNull(result);
                Assert.Equal(publisherId, result.Id);
                Assert.Equal("Test Publisher", result.Name);
            }
        }

        [Fact]
        public async Task AddPublisherAsync_ShouldCreateNewPublisher()
        {
            var opts = CreateNewContextOptions(nameof(AddPublisherAsync_ShouldCreateNewPublisher));
            var newPublisherDTO = new PublisherCreateDTO
            {
                Name = "New Publisher",
                Website = "https://www.newpublisher.com",
                Address = "New Address"
            };

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new PublisherService(new UnitOfWork(context));
                await service.AddPublisherAsync(newPublisherDTO);
            }

            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var publisher = verifyContext.Publishers.SingleOrDefault(p => p.Name == "New Publisher");
                Assert.NotNull(publisher);
                Assert.Equal("https://www.newpublisher.com", publisher.Website);
                Assert.Equal("New Address", publisher.Address);
            }
        }

        [Fact]
        public async Task UpdatePublisherAsync_WithValidId_ShouldUpdatePublisher()
        {
            var opts = CreateNewContextOptions(nameof(UpdatePublisherAsync_WithValidId_ShouldUpdatePublisher));
            var publisherId = Guid.NewGuid();
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Publishers.Add(new Publisher
                {
                    Id = publisherId,
                    Name = "Old Name",
                    Website = "https://www.old.com",
                    Address = "Old Address"
                });
                seedContext.SaveChanges();
            }

            var updateDto = new PublisherUpdateDTO
            {
                Name = "Updated Name",
                Website = "https://www.updated.com",
                Address = "Updated Address"
            };

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new PublisherService(new UnitOfWork(context));
                await service.UpdatePublisherAsync(publisherId, updateDto);
            }

            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var publisher = verifyContext.Publishers.SingleOrDefault(p => p.Id == publisherId);
                Assert.NotNull(publisher);
                Assert.Equal("Updated Name", publisher.Name);
                Assert.Equal("https://www.updated.com", publisher.Website);
                Assert.Equal("Updated Address", publisher.Address);
            }
        }

        [Fact]
        public async Task DeletePublisherAsync_WithValidId_ShouldDeletePublisher()
        {
            var opts = CreateNewContextOptions(nameof(DeletePublisherAsync_WithValidId_ShouldDeletePublisher));
            var publisherId = Guid.NewGuid();
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Publishers.Add(new Publisher { Id = publisherId, Name = "ToDelete", Website = "www.todelete.com", Address = "Delete Address" });
                seedContext.SaveChanges();
            }

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new PublisherService(new UnitOfWork(context));
                await service.DeletePublisherAsync(publisherId);
            }

            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var publisher = verifyContext.Publishers.SingleOrDefault(p => p.Id == publisherId);
                Assert.Null(publisher);
            }
        }

        [Fact]
        public async Task AddPublisherAsync_WithInvalidData_ShouldThrowValidationException()
        {
            var opts = CreateNewContextOptions(nameof(AddPublisherAsync_WithInvalidData_ShouldThrowValidationException));
            var invalidPublisherDTO = new PublisherCreateDTO
            {
                Name = "A", // Too short
                Website = "invalid-url", // Invalid URL
                Address = "A" // Too short
            };

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new PublisherService(new UnitOfWork(context));
                var exception = await Assert.ThrowsAsync<ValidationException>(() => 
                    service.AddPublisherAsync(invalidPublisherDTO));
            }
        }

        [Fact]
        public async Task AddPublisherAsync_WithMissingRequiredFields_ShouldThrowValidationException()
        {
            var opts = CreateNewContextOptions(nameof(AddPublisherAsync_WithMissingRequiredFields_ShouldThrowValidationException));
            var invalidPublisherDTO = new PublisherCreateDTO
            {
                Name = string.Empty,
                Website = string.Empty,
                Address = string.Empty
            };

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new PublisherService(new UnitOfWork(context));
                var exception = await Assert.ThrowsAsync<ValidationException>(() => 
                    service.AddPublisherAsync(invalidPublisherDTO));
            }
        }

        [Fact]
        public async Task UpdatePublisherAsync_WithInvalidData_ShouldThrowValidationException()
        {
            var opts = CreateNewContextOptions(nameof(UpdatePublisherAsync_WithInvalidData_ShouldThrowValidationException));
            var publisherId = Guid.NewGuid();
            var invalidPublisherDTO = new PublisherUpdateDTO
            {
                Name = "A", // Too short
                Website = "invalid-url", // Invalid URL
                Address = "A" // Too short
            };

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new PublisherService(new UnitOfWork(context));
                var exception = await Assert.ThrowsAsync<ValidationException>(() => 
                    service.UpdatePublisherAsync(publisherId, invalidPublisherDTO));
            }
        }
    }
} 