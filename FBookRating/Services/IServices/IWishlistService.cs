using FBookRating.Models.DTOs.WishList;
using FBookRating.Models.Entities;

namespace FBookRating.Services.IServices
{
    public interface IWishlistService
    {
        Task<IEnumerable<WishlistReadDTO>> GetWishlistsByUserAsync(string userId);
        Task AddWishlistAsync(WishlistCreateDTO wishlistDTO, string userId);
        Task AddBookToWishlistAsync(int wishlistId, int bookId);
        Task RemoveBookFromWishlistAsync(int wishlistId, int bookId);
    }
}
