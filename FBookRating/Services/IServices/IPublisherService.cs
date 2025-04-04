using FBookRating.Models.DTOs.Publisher;
using FBookRating.Models.Entities;

namespace FBookRating.Services.IServices
{
    public interface IPublisherService
    {
        Task<IEnumerable<PublisherReadDTO>> GetAllPublishersAsync();
        Task<PublisherReadDTO> GetPublisherByIdAsync(Guid id);
        Task AddPublisherAsync(PublisherCreateDTO publisherCreateDTO);
        Task UpdatePublisherAsync(Guid id, PublisherUpdateDTO publisherUpdateDTO);
        Task DeletePublisherAsync(Guid id);
    }
}
