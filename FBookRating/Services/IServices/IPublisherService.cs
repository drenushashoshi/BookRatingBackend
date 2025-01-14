using FBookRating.Models.DTOs.Publisher;
using FBookRating.Models.Entities;

namespace FBookRating.Services.IServices
{
    public interface IPublisherService
    {
        Task<IEnumerable<PublisherReadDTO>> GetAllPublishersAsync();
        Task<PublisherReadDTO> GetPublisherByIdAsync(int id);
        Task AddPublisherAsync(PublisherCreateDTO publisherCreateDTO);
        Task UpdatePublisherAsync(int id, PublisherUpdateDTO publisherUpdateDTO);
        Task DeletePublisherAsync(int id);
    }
}
