using FBookRating.Models.DTOs.Event;
using FBookRating.Models.Entities;

namespace FBookRating.Services.IServices
{
    public interface IEventService
    {
        Task<IEnumerable<EventReadDTO>> GetAllEventsAsync();
        Task<EventReadDTO> GetEventByIdAsync(int id);
        Task AddEventAsync(EventCreateDTO newEventDTO);
        Task AddBookToEventAsync(int eventId, int bookId);
        Task RemoveBookFromEventAsync(int eventId, int bookId);
        Task DeleteEventAsync(int id);
    }
}
