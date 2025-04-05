using Data_Access_Layer.Entities;

namespace FBookRating.Services.IServices
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetAllTagsAsync();
        Task<Tag> GetTagByIdAsync(Guid id);
        Task AddTagAsync(Tag tag);
        Task UpdateTagAsync(Guid id, Tag tag);
        Task DeleteTagAsync(Guid id);
    }
}
