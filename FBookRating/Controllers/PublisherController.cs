using FBookRating.Models.DTOs.Publisher;
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
    public class PublisherController : ControllerBase
    {
        private readonly IPublisherService _publisherService;

        public PublisherController(IPublisherService publisherService)
        {
            _publisherService = publisherService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPublishers()
        {
            var publishers = await _publisherService.GetAllPublishersAsync();
            return Ok(publishers);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublisherById(Guid id)
        {
            var publisher = await _publisherService.GetPublisherByIdAsync(id);
            if (publisher == null) return NotFound();
            return Ok(publisher);
        }

        [HttpPost]
        public async Task<IActionResult> AddPublisher([FromBody] PublisherCreateDTO publisherCreateDTO)
        {
            await _publisherService.AddPublisherAsync(publisherCreateDTO);
            return Ok("Publisher created successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePublisher(Guid id, [FromBody] PublisherUpdateDTO publisherUpdateDTO)
        {
            await _publisherService.UpdatePublisherAsync(id, publisherUpdateDTO);
            return Ok("Publisher updated successfully.");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(Guid id)
        {
            await _publisherService.DeletePublisherAsync(id);
            return Ok("Publisher deleted successfully.");
        }
    }
}
