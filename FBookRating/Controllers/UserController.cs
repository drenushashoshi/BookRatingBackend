/*
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FBookRating.Models.DTOs.User;

namespace FBookRating.Controllers
{
    [Route("api/user")]
    [ApiController]
    [Authorize] // Requires authentication
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Updates the logged-in user's display name and profile picture.
        /// </summary>
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
        {
            if (model == null)
                return BadRequest("Invalid request data.");

            // Get the logged-in user ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("User not found.");

            // Update the fields
            user.DisplayName = model.DisplayName;
            user.ProfilePictureUrl = model.ProfilePictureUrl;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new
            {
                Message = "Profile updated successfully.",
                DisplayName = user.DisplayName,
                ProfilePictureUrl = user.ProfilePictureUrl
            });
        }
    }

}
*/
