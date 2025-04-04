using FBookRating.DataAccess.UnitOfWork;
using FBookRating.Models.DTOs.ReviewRating;
using FBookRating.Models.Entities;
using FBookRating.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace FBookRating.Services
{
    public class ReviewRatingService : IReviewRatingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewRatingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ReviewRatingReadDTO>> GetReviewsForBookAsync(Guid bookId)
        {
            var reviews = await _unitOfWork.Repository<ReviewRating>()
                .GetByCondition(r => r.BookId == bookId)
                .Include(r => r.User)
                .Include(r => r.Book)
                .ToListAsync();

            return reviews.Select(r => new ReviewRatingReadDTO
            {
                Id = r.Id,
                Score = r.Score,
                ReviewText = r.ReviewText,
                UserName = r.User?.UserName,
                BookTitle = r.Book?.Title
            });
        }

        public async Task AddReviewAsync(ReviewRatingCreateDTO reviewRatingDTO, string userId)
        {
            var reviewRating = new ReviewRating
            {
                Score = reviewRatingDTO.Score,
                ReviewText = reviewRatingDTO.ReviewText,
                BookId = reviewRatingDTO.BookId,
                UserId = userId
            };

            _unitOfWork.Repository<ReviewRating>().Create(reviewRating);
            await _unitOfWork.Repository<ReviewRating>().SaveChangesAsync();
        }

        public async Task<double> GetAverageRatingForBookAsync(Guid bookId)
        {
            var ratings = await _unitOfWork.Repository<ReviewRating>()
                .GetByCondition(r => r.BookId == bookId)
                .Select(r => r.Score)
                .ToListAsync();

            return ratings.Any() ? ratings.Average() : 0.0;
        }
    }
}
