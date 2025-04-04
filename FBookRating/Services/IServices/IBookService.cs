using FBookRating.Models.DTOs.Book;
using FBookRating.Models.Entities;

namespace FBookRating.Services.IServices
{
    public interface IBookService
    {
        Task<IEnumerable<BookReadDTO>> GetAllBooksAsync();
        Task<BookReadDTO> GetBookByIdAsync(Guid id);
        Task AddBookAsync(BookCreateDTO bookCreateDTO);
        Task UpdateBookAsync(Guid id, BookUpdateDTO bookUpdateDTO);
        Task DeleteBookAsync(Guid id);
    }
}
