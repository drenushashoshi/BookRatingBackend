using FBookRating.Models.DTOs.Book;
using FBookRating.Models.Entities;

namespace FBookRating.Services.IServices
{
    public interface IBookService
    {
        Task<IEnumerable<BookReadDTO>> GetAllBooksAsync();
        Task<BookReadDTO> GetBookByIdAsync(int id);
        Task AddBookAsync(BookCreateDTO bookCreateDTO);
        Task UpdateBookAsync(int id, BookUpdateDTO bookUpdateDTO);
        Task DeleteBookAsync(int id);
    }
}
