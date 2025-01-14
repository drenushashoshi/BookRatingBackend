using FBookRating.DataAccess.UnitOfWork;
using FBookRating.Models.DTOs.Publisher;
using FBookRating.Models.Entities;
using FBookRating.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace FBookRating.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PublisherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PublisherReadDTO>> GetAllPublishersAsync()
        {
            var publishers = await _unitOfWork.Repository<Publisher>().GetAll().ToListAsync();
            return publishers.Select(p => new PublisherReadDTO
            {
                Id = p.Id,
                Name = p.Name,
                Website = p.Website,
                Address = p.Address
            });
        }

        public async Task<PublisherReadDTO> GetPublisherByIdAsync(int id)
        {
            var publisher = await _unitOfWork.Repository<Publisher>().GetByCondition(p => p.Id == id).FirstOrDefaultAsync();
            if (publisher == null) return null;

            return new PublisherReadDTO
            {
                Id = publisher.Id,
                Name = publisher.Name,
                Website = publisher.Website,
                Address = publisher.Address
            };
        }

        public async Task AddPublisherAsync(PublisherCreateDTO publisherCreateDTO)
        {
            var publisher = new Publisher
            {
                Name = publisherCreateDTO.Name,
                Website = publisherCreateDTO.Website,
                Address = publisherCreateDTO.Address
            };

            _unitOfWork.Repository<Publisher>().Create(publisher);
            await _unitOfWork.Repository<Publisher>().SaveChangesAsync();
        }

        public async Task UpdatePublisherAsync(int id, PublisherUpdateDTO publisherUpdateDTO)
        {
            var existingPublisher = await _unitOfWork.Repository<Publisher>().GetByCondition(p => p.Id == id).FirstOrDefaultAsync();
            if (existingPublisher == null) throw new Exception("Publisher not found.");

            existingPublisher.Name = publisherUpdateDTO.Name;
            existingPublisher.Website = publisherUpdateDTO.Website;
            existingPublisher.Address = publisherUpdateDTO.Address;

            _unitOfWork.Repository<Publisher>().Update(existingPublisher);
            await _unitOfWork.Repository<Publisher>().SaveChangesAsync();
        }

        public async Task DeletePublisherAsync(int id)
        {
            var publisher = await _unitOfWork.Repository<Publisher>().GetByCondition(p => p.Id == id).FirstOrDefaultAsync();
            if (publisher != null)
            {
                _unitOfWork.Repository<Publisher>().Delete(publisher);
                await _unitOfWork.Repository<Publisher>().SaveChangesAsync();
            }
        }
    }
}
