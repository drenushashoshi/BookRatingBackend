using FBookRating.Models.DTOs.Author;
using FBookRating.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBookRating.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorService _authorService;

        public AuthorController(IAuthorService authorService)
        {
            _authorService = authorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAuthors()
        {
            var authors = await _authorService.GetAllAuthorsAsync();
            return Ok(authors);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            var author = await _authorService.GetAuthorByIdAsync(id);
            if (author == null) return NotFound();
            return Ok(author);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddAuthor([FromBody] AuthorCreateDTO authorDTO)
        {
            await _authorService.AddAuthorAsync(authorDTO);
            return NoContent();
        }

        /*
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, [FromBody] Author author)
        {
            if (id != author.Id) return BadRequest("ID mismatch");
            await _authorService.UpdateAuthorAsync(author);
            return NoContent();
        }
        

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            await _authorService.DeleteAuthorAsync(id);
            return NoContent();
        }

        */
    }
}
