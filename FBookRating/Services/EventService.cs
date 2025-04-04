using FBookRating.DataAccess.UnitOfWork;
using FBookRating.Models.DTOs.Event;
using FBookRating.Models.Entities;
using FBookRating.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace FBookRating.Services
{
    public class EventService : IEventService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EventService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<EventReadDTO>> GetAllEventsAsync()
        {
            var events = await _unitOfWork.Repository<Event>()
                .GetAll()
                .Include(e => e.BookEvents)
                .ThenInclude(be => be.Book)
                .ToListAsync();

            return events.Select(e => new EventReadDTO
            {
                Id = e.Id,
                Name = e.Name,
                Location = e.Location,
                StartDate = e.StartDate,
                Description = e.Description,
                Books = e.BookEvents.Select(be => new BookEventDTO
                {
                    EventId = be.EventId,
                    BookId = be.BookId,
                    BookTitle = be.Book.Title
                })
            });
        }

        public async Task<EventReadDTO> GetEventByIdAsync(Guid id)
        {
            var eventEntity = await _unitOfWork.Repository<Event>()
                .GetByCondition(e => e.Id == id)
                .Include(e => e.BookEvents)
                .ThenInclude(be => be.Book)
                .FirstOrDefaultAsync();

            if (eventEntity == null) return null;

            return new EventReadDTO
            {
                Id = eventEntity.Id,
                Name = eventEntity.Name,
                Location = eventEntity.Location,
                StartDate = eventEntity.StartDate,
                Description = eventEntity.Description,
                Books = eventEntity.BookEvents.Select(be => new BookEventDTO
                {
                    EventId = be.EventId,
                    BookId = be.BookId,
                    BookTitle = be.Book.Title
                })
            };
        }

        public async Task AddEventAsync(EventCreateDTO newEventDTO)
        {
            var eventEntity = new Event
            {
                Name = newEventDTO.Name,
                Location = newEventDTO.Location,
                StartDate = newEventDTO.StartDate,
                Description = newEventDTO.Description
            };

            _unitOfWork.Repository<Event>().Create(eventEntity);
            await _unitOfWork.Repository<Event>().SaveChangesAsync();
        }

        public async Task AddBookToEventAsync(Guid eventId, Guid bookId)
        {
            var bookEvent = new BookEvent { EventId = eventId, BookId = bookId };
            _unitOfWork.Repository<BookEvent>().Create(bookEvent);
            await _unitOfWork.Repository<BookEvent>().SaveChangesAsync();
        }

        public async Task RemoveBookFromEventAsync(Guid eventId, Guid bookId)
        {
            var bookEvent = await _unitOfWork.Repository<BookEvent>()
                .GetByCondition(be => be.EventId == eventId && be.BookId == bookId)
                .FirstOrDefaultAsync();

            if (bookEvent != null)
            {
                _unitOfWork.Repository<BookEvent>().Delete(bookEvent);
                await _unitOfWork.Repository<BookEvent>().SaveChangesAsync();
            }
        }

        public async Task DeleteEventAsync(Guid id)
        {
            var eventEntity = await _unitOfWork.Repository<Event>()
                .GetByCondition(e => e.Id == id)
                .FirstOrDefaultAsync();

            if (eventEntity != null)
            {
                _unitOfWork.Repository<Event>().Delete(eventEntity);
                await _unitOfWork.Repository<Event>().SaveChangesAsync();
            }
        }
    }
}
