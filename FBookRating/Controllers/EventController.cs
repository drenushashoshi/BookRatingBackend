using FBookRating.Models.DTOs.Event;
using FBookRating.Models.Entities;
using FBookRating.Services;
using FBookRating.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBookRating.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventService.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventEntity = await _eventService.GetEventByIdAsync(id);
            if (eventEntity == null) return NotFound();
            return Ok(eventEntity);
        }

        [HttpPost]
        public async Task<IActionResult> AddEvent([FromBody] EventCreateDTO newEventDTO)
        {
            await _eventService.AddEventAsync(newEventDTO);
            return Created("", "Event created successfully.");
        }

        [HttpPost("{eventId}/books/{bookId}")]
        public async Task<IActionResult> AddBookToEvent(int eventId, int bookId)
        {
            await _eventService.AddBookToEventAsync(eventId, bookId);
            return NoContent();
        }

        [HttpDelete("{eventId}/books/{bookId}")]
        public async Task<IActionResult> RemoveBookFromEvent(int eventId, int bookId)
        {
            await _eventService.RemoveBookFromEventAsync(eventId, bookId);
            return NoContent();
        }
    }

}
