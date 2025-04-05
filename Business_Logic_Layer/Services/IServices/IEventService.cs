using FBookRating.Models.DTOs.Event;

namespace FBookRating.Services.IServices
{
    public interface IEventService
    {
        Task<IEnumerable<EventReadDTO>> GetAllEventsAsync();
        Task<EventReadDTO> GetEventByIdAsync(Guid id);
        Task AddEventAsync(EventCreateDTO newEventDTO);
        Task AddBookToEventAsync(Guid eventId, Guid bookId);
        Task RemoveBookFromEventAsync(Guid eventId, Guid bookId);
        Task DeleteEventAsync(Guid id);
    }
}
