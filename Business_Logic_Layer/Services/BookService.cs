using Data_Access_Layer.Entities;
using Data_Access_Layer.UnitOfWork;
using FBookRating.Models.DTOs.Book;
using FBookRating.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace FBookRating.Services
{
    [Authorize(Roles = "Admin")]
    public class BookService : IBookService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageUploadService _imageUploadService;

        public BookService(IUnitOfWork unitOfWork, IImageUploadService imageUploadService)
        {
            _unitOfWork = unitOfWork;
            _imageUploadService = imageUploadService;
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

        public async Task<BookReadDTO> GetBookByIdAsync(Guid id)
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
            string imageUrl = null;
            if (bookCreateDTO.CoverImage != null)
            {
                imageUrl = await _imageUploadService.UploadImageAsync(bookCreateDTO.CoverImage);
            }

            var book = new Book
            {
                Title = bookCreateDTO.Title,
                ISBN = bookCreateDTO.ISBN,
                Description = bookCreateDTO.Description,
                PublishedDate = bookCreateDTO.PublishedDate,
                CoverImageUrl = imageUrl,
                CategoryId = bookCreateDTO.CategoryId,
                AuthorId = bookCreateDTO.AuthorId,
                PublisherId = bookCreateDTO.PublisherId
            };

            _unitOfWork.Repository<Book>().Create(book);
            await _unitOfWork.Repository<Book>().SaveChangesAsync();
        }

        public async Task UpdateBookAsync(Guid id, BookUpdateDTO bookUpdateDTO)
        {
            var existingBook = await _unitOfWork.Repository<Book>().GetByCondition(b => b.Id == id).FirstOrDefaultAsync();
            if (existingBook == null) throw new Exception("Book not found.");

            string imageUrl = existingBook.CoverImageUrl;
            if (bookUpdateDTO.CoverImage != null)
            {
                imageUrl = await _imageUploadService.UploadImageAsync(bookUpdateDTO.CoverImage);
            }

            existingBook.Title = bookUpdateDTO.Title;
            existingBook.ISBN = bookUpdateDTO.ISBN;
            existingBook.Description = bookUpdateDTO.Description;
            existingBook.PublishedDate = bookUpdateDTO.PublishedDate;
            existingBook.CoverImageUrl = imageUrl;
            existingBook.CategoryId = bookUpdateDTO.CategoryId;
            existingBook.AuthorId = bookUpdateDTO.AuthorId;
            existingBook.PublisherId = bookUpdateDTO.PublisherId;

            _unitOfWork.Repository<Book>().Update(existingBook);
            await _unitOfWork.Repository<Book>().SaveChangesAsync();
        }

        public async Task DeleteBookAsync(Guid id)
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
