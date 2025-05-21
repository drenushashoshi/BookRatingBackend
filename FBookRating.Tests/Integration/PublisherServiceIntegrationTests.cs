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

namespace FBookRating.Tests.Integration
{
    public class PublisherServiceIntegrationTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly PublisherService _service;
        private readonly IUnitOfWork _unitOfWork;

        public PublisherServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "PublisherServiceIntegrationTests")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
            _unitOfWork = new UnitOfWork(_context);
            _service = new PublisherService(_unitOfWork);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task PublisherService_CompleteFlow_ShouldWork()
        {
            // Arrange
            var createDto = new PublisherCreateDTO
            {
                Name = "Test Publisher",
                Website = "https://www.testpublisher.com",
                Address = "123 Test St"
            };

            // Act - Create
            await _service.AddPublisherAsync(createDto);
            var createdPublisher = await _context.Publishers.FirstOrDefaultAsync(p => p.Name == createDto.Name);

            // Assert - Create
            Assert.NotNull(createdPublisher);
            Assert.Equal(createDto.Name, createdPublisher.Name);
            Assert.Equal(createDto.Website, createdPublisher.Website);
            Assert.Equal(createDto.Address, createdPublisher.Address);

            // Act - Update
            var updateDto = new PublisherUpdateDTO
            {
                Name = "Updated Publisher",
                Website = "https://www.updatedpublisher.com",
                Address = "456 Updated St"
            };
            await _service.UpdatePublisherAsync(createdPublisher.Id, updateDto);

            // Assert - Update
            var updatedPublisher = await _service.GetPublisherByIdAsync(createdPublisher.Id);
            Assert.NotNull(updatedPublisher);
            Assert.Equal(updateDto.Name, updatedPublisher.Name);
            Assert.Equal(updateDto.Website, updatedPublisher.Website);
            Assert.Equal(updateDto.Address, updatedPublisher.Address);

            // Act - Delete
            await _service.DeletePublisherAsync(createdPublisher.Id);

            // Assert - Delete
            var deletedPublisher = await _service.GetPublisherByIdAsync(createdPublisher.Id);
            Assert.Null(deletedPublisher);
        }

        [Fact]
        public async Task PublisherService_WithInvalidData_ShouldThrowValidationException()
        {
            // Arrange
            var invalidDto = new PublisherCreateDTO
            {
                Name = "A", // Too short
                Website = "invalid-url", // Invalid URL
                Address = "A" // Too short
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.AddPublisherAsync(invalidDto));
        }

        [Fact]
        public async Task PublisherService_WithDuplicateName_ShouldCreateSuccessfully()
        {
            // Arrange
            var publisher1 = new PublisherCreateDTO
            {
                Name = "Same Name Publisher",
                Website = "https://www.publisher1.com",
                Address = "Address 1"
            };

            var publisher2 = new PublisherCreateDTO
            {
                Name = "Same Name Publisher",
                Website = "https://www.publisher2.com",
                Address = "Address 2"
            };

            // Act
            await _service.AddPublisherAsync(publisher1);
            await _service.AddPublisherAsync(publisher2);

            // Assert
            var publishers = await _service.GetAllPublishersAsync();
            Assert.Equal(2, publishers.Count(p => p.Name == "Same Name Publisher"));
        }
    }
} 