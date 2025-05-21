using Xunit;
using Data_Access_Layer.Entities;
using Data_Access_Layer.UnitOfWork;
using FBookRating.Services;
using FBookRating.Models.DTOs.Event;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Linq;
using Data_Access_Layer;
using System.ComponentModel.DataAnnotations;

namespace FBookRating.Tests.Integration
{
    public class EventServiceIntegrationTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly EventService _service;
        private readonly IUnitOfWork _unitOfWork;

        public EventServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "EventServiceIntegrationTests")
                .Options;

            _context = new ApplicationDbContext(options);
            _context.Database.EnsureCreated();
            _unitOfWork = new UnitOfWork(_context);
            _service = new EventService(_unitOfWork);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task EventService_CompleteFlow_ShouldWork()
        {
            // Arrange
            var createDto = new EventCreateDTO
            {
                Name = "Test Event",
                Description = "Test Description",
                StartDate = DateTime.UtcNow.AddDays(1),
                Location = "Test Location"
            };

            // Act - Create
            await _service.AddEventAsync(createDto);
            var createdEvent = await _context.Events.FirstOrDefaultAsync(e => e.Name == createDto.Name);

            // Assert - Create
            Assert.NotNull(createdEvent);
            Assert.Equal(createDto.Name, createdEvent.Name);
            Assert.Equal(createDto.Description, createdEvent.Description);
            Assert.Equal(createDto.Location, createdEvent.Location);

            // Act - Delete
            await _service.DeleteEventAsync(createdEvent.Id);

            // Assert - Delete
            var deletedEvent = await _service.GetEventByIdAsync(createdEvent.Id);
            Assert.Null(deletedEvent);
        }

        [Fact]
        public async Task EventService_WithInvalidData_ShouldThrowValidationException()
        {
            // Arrange
            var invalidDto = new EventCreateDTO
            {
                Name = "A", // Too short
                Description = "A", // Too short
                StartDate = DateTime.UtcNow,
                Location = "A" // Too short
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => _service.AddEventAsync(invalidDto));
        }

        [Fact]
        public async Task EventService_AddAndRemoveBook_ShouldWork()
        {
            // Arrange
            var eventDto = new EventCreateDTO
            {
                Name = "Test Event",
                Description = "Test Description",
                StartDate = DateTime.UtcNow.AddDays(1),
                Location = "Test Location"
            };

            // Create event
            await _service.AddEventAsync(eventDto);
            var createdEvent = await _context.Events.FirstOrDefaultAsync(e => e.Name == eventDto.Name);
            Assert.NotNull(createdEvent);

            // Create a book
            var book = new Book
            {
                Title = "Test Book",
                ISBN = "9781234567890",
                Description = "Test Description",
                PublishedDate = DateTime.UtcNow,
                CoverImageUrl = "https://example.com/image.jpg"
            };
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act - Add book to event
            await _service.AddBookToEventAsync(createdEvent.Id, book.Id);

            // Assert - Book is added
            var eventWithBook = await _service.GetEventByIdAsync(createdEvent.Id);
            Assert.NotNull(eventWithBook);
            Assert.NotNull(eventWithBook.Books);
            Assert.Single(eventWithBook.Books);
            Assert.Equal(book.Id, eventWithBook.Books.First().BookId);

            // Act - Remove book from event
            await _service.RemoveBookFromEventAsync(createdEvent.Id, book.Id);

            // Assert - Book is removed
            var eventWithoutBook = await _service.GetEventByIdAsync(createdEvent.Id);
            Assert.NotNull(eventWithoutBook);
            Assert.NotNull(eventWithoutBook.Books);
            Assert.Empty(eventWithoutBook.Books);
        }

        [Fact]
        public async Task EventService_GetAllEvents_ShouldReturnAllEvents()
        {
            // Arrange
            var event1 = new EventCreateDTO
            {
                Name = "Event 1",
                Description = "Description 1",
                StartDate = DateTime.UtcNow.AddDays(1),
                Location = "Location 1"
            };

            var event2 = new EventCreateDTO
            {
                Name = "Event 2",
                Description = "Description 2",
                StartDate = DateTime.UtcNow.AddDays(2),
                Location = "Location 2"
            };

            await _service.AddEventAsync(event1);
            await _service.AddEventAsync(event2);

            // Act
            var events = await _service.GetAllEventsAsync();

            // Assert
            Assert.Equal(2, events.Count());
            var eventList = events.ToList();
            Assert.Contains(eventList, e => e.Name == event1.Name);
            Assert.Contains(eventList, e => e.Name == event2.Name);
        }
    }
} 