using FBookRating.DataAccess.UnitOfWork;
using FBookRating.Models.DTOs.WishList;
using FBookRating.Models.Entities;
using FBookRating.Services.IServices;
using Microsoft.EntityFrameworkCore;

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
            var wishlist = new Wishlist
            {
                Name = wishlistDTO.Name,
                UserId = userId
            };

            _unitOfWork.Repository<Wishlist>().Create(wishlist);
            await _unitOfWork.Repository<Wishlist>().SaveChangesAsync();
        }

        public async Task AddBookToWishlistAsync(int wishlistId, int bookId)
        {
            var wishlistBookExists = await _unitOfWork.Repository<WishlistBook>()
                .GetByCondition(wb => wb.WishlistId == wishlistId && wb.BookId == bookId)
                .AnyAsync();

            if (!wishlistBookExists)
            {
                var wishlistBook = new WishlistBook
                {
                    WishlistId = wishlistId,
                    BookId = bookId,
                    AddedDate = DateTime.UtcNow
                };

                _unitOfWork.Repository<WishlistBook>().Create(wishlistBook);
                await _unitOfWork.Repository<WishlistBook>().SaveChangesAsync();
            }
        }

        public async Task RemoveBookFromWishlistAsync(int wishlistId, int bookId)
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
