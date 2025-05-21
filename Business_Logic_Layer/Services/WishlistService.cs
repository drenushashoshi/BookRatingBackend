using Data_Access_Layer.Entities;
using Data_Access_Layer.UnitOfWork;
using FBookRating.Models.DTOs.WishList;
using FBookRating.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FBookRating.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WishlistService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<WishlistReadDTO>> GetWishlistsByUserAsync(string userId)
        {
            var wishlists = await _unitOfWork.Repository<Wishlist>()
                .GetByCondition(w => w.UserId == userId)
                .Include(w => w.WishlistBooks)
                .ThenInclude(wb => wb.Book)
                .ToListAsync();

            return wishlists.Select(w => new WishlistReadDTO
            {
                Id = w.Id,
                Name = w.Name,
                UserId = w.UserId,
                Books = w.WishlistBooks.Select(wb => new WishlistBookDTO
                {
                    WishlistId = wb.WishlistId,
                    BookId = wb.BookId,
                    BookTitle = wb.Book.Title,
                    AddedDate = wb.AddedDate
                })
            });
        }

        public async Task AddWishlistAsync(WishlistCreateDTO wishlistDTO, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ValidationException("User ID is required");
            }

            var validationContext = new ValidationContext(wishlistDTO);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(wishlistDTO, validationContext, validationResults, true))
            {
                throw new ValidationException(validationResults.First().ErrorMessage);
            }

            var wishlist = new Wishlist
            {
                Name = wishlistDTO.Name,
                UserId = userId
            };

            _unitOfWork.Repository<Wishlist>().Create(wishlist);
            await _unitOfWork.Repository<Wishlist>().SaveChangesAsync();
        }

        public async Task AddBookToWishlistAsync(Guid wishlistId, Guid bookId)
        {
            if (wishlistId == Guid.Empty || bookId == Guid.Empty)
            {
                throw new ValidationException("Invalid Wishlist ID or Book ID");
            }

            var existingBook = await _unitOfWork.Repository<WishlistBook>()
                .GetByCondition(wb => wb.WishlistId == wishlistId && wb.BookId == bookId)
                .FirstOrDefaultAsync();

            if (existingBook != null)
            {
                throw new ValidationException("Book is already in the wishlist");
            }

            var bookEvent = new WishlistBook { WishlistId = wishlistId, BookId = bookId, AddedDate = DateTime.UtcNow };
            _unitOfWork.Repository<WishlistBook>().Create(bookEvent);
            await _unitOfWork.Repository<WishlistBook>().SaveChangesAsync();
        }

        public async Task RemoveBookFromWishlistAsync(Guid wishlistId, Guid bookId)
        {
            var wishlistBook = await _unitOfWork.Repository<WishlistBook>()
                .GetByCondition(wb => wb.WishlistId == wishlistId && wb.BookId == bookId)
                .FirstOrDefaultAsync();

            if (wishlistBook != null)
            {
                _unitOfWork.Repository<WishlistBook>().Delete(wishlistBook);
                await _unitOfWork.Repository<WishlistBook>().SaveChangesAsync();
            }
        }
    }
}
