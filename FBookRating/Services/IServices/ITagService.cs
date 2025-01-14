using FBookRating.Models.Entities;

namespace FBookRating.Services.IServices
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetAllTagsAsync();
        Task<Tag> GetTagByIdAsync(int id);
        Task AddTagAsync(Tag tag);
        Task UpdateTagAsync(int id, Tag tag);
        Task DeleteTagAsync(int id);
    }
}
