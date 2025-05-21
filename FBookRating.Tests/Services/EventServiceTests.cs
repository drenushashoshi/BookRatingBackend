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

namespace FBookRating.Tests.Services
{
    public class EventServiceTests
    {
        private DbContextOptions<ApplicationDbContext> CreateNewContextOptions(string dbName)
            => new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

        [Fact]
        public async Task GetAllEventsAsync_ShouldReturnAllEvents()
        {
            var opts = CreateNewContextOptions(nameof(GetAllEventsAsync_ShouldReturnAllEvents));
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Events.AddRange(
                    new Event { Id = Guid.NewGuid(), Name = "Event 1", Location = "Location 1", StartDate = DateTime.Now, Description = "Description 1" },
                    new Event { Id = Guid.NewGuid(), Name = "Event 2", Location = "Location 2", StartDate = DateTime.Now, Description = "Description 2" }
                );
                seedContext.SaveChanges();
            }

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new EventService(new UnitOfWork(context));
                var result = await service.GetAllEventsAsync();
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetEventByIdAsync_WithValidId_ShouldReturnEvent()
        {
            var opts = CreateNewContextOptions(nameof(GetEventByIdAsync_WithValidId_ShouldReturnEvent));
            var eventId = Guid.NewGuid();
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Events.Add(new Event
                {
                    Id = eventId,
                    Name = "Test Event",
                    Location = "Test Location",
                    StartDate = DateTime.Now,
                    Description = "Test Description"
                });
                seedContext.SaveChanges();
            }

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new EventService(new UnitOfWork(context));
                var result = await service.GetEventByIdAsync(eventId);
                Assert.NotNull(result);
                Assert.Equal(eventId, result.Id);
                Assert.Equal("Test Event", result.Name);
            }
        }

        [Fact]
        public async Task AddEventAsync_ShouldCreateNewEvent()
        {
            var opts = CreateNewContextOptions(nameof(AddEventAsync_ShouldCreateNewEvent));
            var newEventDTO = new EventCreateDTO
            {
                Name = "New Event",
                Location = "New Location",
                StartDate = DateTime.Now,
                Description = "New Description"
            };

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new EventService(new UnitOfWork(context));
                await service.AddEventAsync(newEventDTO);
            }

            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var ev = verifyContext.Events.SingleOrDefault(e => e.Name == "New Event");
                Assert.NotNull(ev);
                Assert.Equal("New Location", ev.Location);
                Assert.Equal("New Description", ev.Description);
            }
        }

        [Fact]
        public async Task AddBookToEventAsync_ShouldCreateBookEvent()
        {
            var opts = CreateNewContextOptions(nameof(AddBookToEventAsync_ShouldCreateBookEvent));
            var eventId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Events.Add(new Event {
                    Id = eventId,
                    Name = "Event",
                    Description = "desc",
                    Location = "loc",
                    StartDate = DateTime.UtcNow
                });
                seedContext.Books.Add(new Book {
                    Id = bookId,
                    Title = "Book",
                    CoverImageUrl = "cover.jpg",
                    Description = "desc",
                    ISBN = "isbn",
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = Guid.NewGuid()
                });
                seedContext.SaveChanges();
            }

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new EventService(new UnitOfWork(context));
                await service.AddBookToEventAsync(eventId, bookId);
            }

            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var bookEvent = verifyContext.BookEvents.SingleOrDefault(be => be.EventId == eventId && be.BookId == bookId);
                Assert.NotNull(bookEvent);
            }
        }

        [Fact]
        public async Task RemoveBookFromEventAsync_ShouldDeleteBookEvent()
        {
            var opts = CreateNewContextOptions(nameof(RemoveBookFromEventAsync_ShouldDeleteBookEvent));
            var eventId = Guid.NewGuid();
            var bookId = Guid.NewGuid();
            using (var seedContext = new ApplicationDbContext(opts))
            {
                seedContext.Events.Add(new Event {
                    Id = eventId,
                    Name = "Event",
                    Description = "desc",
                    Location = "loc",
                    StartDate = DateTime.UtcNow
                });
                seedContext.Books.Add(new Book {
                    Id = bookId,
                    Title = "Book",
                    CoverImageUrl = "cover.jpg",
                    Description = "desc",
                    ISBN = "isbn",
                    PublishedDate = DateTime.UtcNow,
                    CategoryId = Guid.NewGuid()
                });
                seedContext.BookEvents.Add(new BookEvent { EventId = eventId, BookId = bookId });
                seedContext.SaveChanges();
            }

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new EventService(new UnitOfWork(context));
                await service.RemoveBookFromEventAsync(eventId, bookId);
            }

            using (var verifyContext = new ApplicationDbContext(opts))
            {
                var bookEvent = verifyContext.BookEvents.SingleOrDefault(be => be.EventId == eventId && be.BookId == bookId);
                Assert.Null(bookEvent);
            }
        }

        [Fact]
        public async Task AddEventAsync_WithInvalidData_ShouldThrowValidationException()
        {
            var opts = CreateNewContextOptions(nameof(AddEventAsync_WithInvalidData_ShouldThrowValidationException));
            var invalidEventDTO = new EventCreateDTO
            {
                Name = "A", // Too short
                Location = "L", // Too short
                StartDate = DateTime.Now,
                Description = "Too short" // Too short
            };

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new EventService(new UnitOfWork(context));
                var exception = await Assert.ThrowsAsync<ValidationException>(() => 
                    service.AddEventAsync(invalidEventDTO));
            }
        }

        [Fact]
        public async Task AddEventAsync_WithMissingRequiredFields_ShouldThrowValidationException()
        {
            var opts = CreateNewContextOptions(nameof(AddEventAsync_WithMissingRequiredFields_ShouldThrowValidationException));
            var invalidEventDTO = new EventCreateDTO
            {
                Name = null,
                Location = null,
                StartDate = DateTime.Now,
                Description = null
            };

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new EventService(new UnitOfWork(context));
                var exception = await Assert.ThrowsAsync<ValidationException>(() => 
                    service.AddEventAsync(invalidEventDTO));
            }
        }

        [Fact]
        public async Task AddBookToEventAsync_WithInvalidIds_ShouldThrowValidationException()
        {
            var opts = CreateNewContextOptions(nameof(AddBookToEventAsync_WithInvalidIds_ShouldThrowValidationException));
            var invalidEventId = Guid.Empty;
            var invalidBookId = Guid.Empty;

            using (var context = new ApplicationDbContext(opts))
            {
                var service = new EventService(new UnitOfWork(context));
                var exception = await Assert.ThrowsAsync<ValidationException>(() => 
                    service.AddBookToEventAsync(invalidEventId, invalidBookId));
            }
        }
    }
} 