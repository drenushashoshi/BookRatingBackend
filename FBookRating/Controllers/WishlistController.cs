using FBookRating.Models.DTOs.WishList;
using FBookRating.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FBookRating.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserWishlists()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var wishlists = await _wishlistService.GetWishlistsByUserAsync(userId);
            return Ok(wishlists);
        }

        [HttpPost]
        public async Task<IActionResult> AddWishlist([FromBody] WishlistCreateDTO wishlistDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _wishlistService.AddWishlistAsync(wishlistDTO, userId);
            return Created("", "Wishlist created successfully.");
        }

        [HttpPost("{wishlistId}/books/{bookId}")]
        public async Task<IActionResult> AddBookToWishlist(Guid wishlistId, Guid bookId)
        {
            await _wishlistService.AddBookToWishlistAsync(wishlistId, bookId);
            return NoContent();
        }

        [HttpDelete("{wishlistId}/books/{bookId}")]
        public async Task<IActionResult> RemoveBookFromWishlist(Guid wishlistId, Guid bookId)
        {
            await _wishlistService.RemoveBookFromWishlistAsync(wishlistId, bookId);
            return NoContent();
        }
    }
}
