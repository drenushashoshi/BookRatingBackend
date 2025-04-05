using FBookRating.Models.DTOs.ReviewRating;
using FBookRating.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FBookRating.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewRatingController : ControllerBase
    {
        private readonly IReviewRatingService _reviewRatingService;

        public ReviewRatingController(IReviewRatingService reviewRatingService)
        {
            _reviewRatingService = reviewRatingService;
        }

        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetReviewsForBook(Guid bookId)
        {
            var reviews = await _reviewRatingService.GetReviewsForBookAsync(bookId);
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> AddReview([FromBody] ReviewRatingCreateDTO reviewRatingDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _reviewRatingService.AddReviewAsync(reviewRatingDTO, userId);
            return Created("", "Review added successfully.");
        }

        [HttpGet("book/{bookId}/average")]
        public async Task<IActionResult> GetAverageRatingForBook(Guid bookId)
        {
            var averageRating = await _reviewRatingService.GetAverageRatingForBookAsync(bookId);
            return Ok(new { AverageRating = averageRating });
        }
    }
}
