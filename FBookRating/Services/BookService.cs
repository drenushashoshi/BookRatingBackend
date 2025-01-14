using FBookRating.DataAccess.UnitOfWork;
using FBookRating.Models.DTOs.Book;
using FBookRating.Models.Entities;
using FBookRating.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace FBookRating.Services
{
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<BookReadDTO>> GetAllBooksAsync()
        {
            var books = await _unitOfWork.Repository<Book>()
                .GetAll()
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .ToListAsync();

            return books.Select(b => new BookReadDTO
            {
                Id = b.Id,
                Title = b.Title,
                ISBN = b.ISBN,
                Description = b.Description,
                PublishedDate = b.PublishedDate,
                CoverImageUrl = b.CoverImageUrl,
                AuthorName = b.Author?.Name,
                PublisherName = b.Publisher?.Name,
                CategoryName = b.Category?.Name
            });
        }

        public async Task<BookReadDTO> GetBookByIdAsync(int id)
        {
            var book = await _unitOfWork.Repository<Book>()
                .GetByCondition(b => b.Id == id)
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Category)
                .FirstOrDefaultAsync();

            if (book == null) return null;

            return new BookReadDTO
            {
                Id = book.Id,
                Title = book.Title,
                ISBN = book.ISBN,
                Description = book.Description,
                PublishedDate = book.PublishedDate,
                CoverImageUrl = book.CoverImageUrl,
                AuthorName = book.Author?.Name,
                PublisherName = book.Publisher?.Name,
                CategoryName = book.Category?.Name
            };
        }

        public async Task AddBookAsync(BookCreateDTO bookCreateDTO)
        {
            var book = new Book
            {
                Title = bookCreateDTO.Title,
                ISBN = bookCreateDTO.ISBN,
                Description = bookCreateDTO.Description,
                PublishedDate = bookCreateDTO.PublishedDate,
                CoverImageUrl = bookCreateDTO.CoverImageUrl,
                CategoryId = bookCreateDTO.CategoryId,
                AuthorId = bookCreateDTO.AuthorId,
                PublisherId = bookCreateDTO.PublisherId
            };

            _unitOfWork.Repository<Book>().Create(book);
            await _unitOfWork.Repository<Book>().SaveChangesAsync();
        }

        public async Task UpdateBookAsync(int id, BookUpdateDTO bookUpdateDTO)
        {
            var existingBook = await _unitOfWork.Repository<Book>().GetByCondition(b => b.Id == id).FirstOrDefaultAsync();
            if (existingBook == null) throw new Exception("Book not found.");

            existingBook.Title = bookUpdateDTO.Title;
            existingBook.ISBN = bookUpdateDTO.ISBN;
            existingBook.Description = bookUpdateDTO.Description;
            existingBook.PublishedDate = bookUpdateDTO.PublishedDate;
            existingBook.CoverImageUrl = bookUpdateDTO.CoverImageUrl;
            existingBook.CategoryId = bookUpdateDTO.CategoryId;
            existingBook.AuthorId = bookUpdateDTO.AuthorId;
            existingBook.PublisherId = bookUpdateDTO.PublisherId;

            _unitOfWork.Repository<Book>().Update(existingBook);
            await _unitOfWork.Repository<Book>().SaveChangesAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _unitOfWork.Repository<Book>().GetByCondition(b => b.Id == id).FirstOrDefaultAsync();
            if (book != null)
            {
                _unitOfWork.Repository<Book>().Delete(book);
                await _unitOfWork.Repository<Book>().SaveChangesAsync();
            }
        }
    }
}
