using FBookRating.Models.DTOs.Book;
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
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await _bookService.GetAllBooksAsync();
            return Ok(books);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBookById(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpPost]
        public async Task<IActionResult> AddBook([FromBody] BookCreateDTO bookCreateDTO)
        {
            await _bookService.AddBookAsync(bookCreateDTO);
            return Ok("Book created successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] BookUpdateDTO bookUpdateDTO)
        {
            await _bookService.UpdateBookAsync(id, bookUpdateDTO);
            return Ok("Book updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            await _bookService.DeleteBookAsync(id);
            return Ok("Book deleted successfully.");
        }
    }
}
