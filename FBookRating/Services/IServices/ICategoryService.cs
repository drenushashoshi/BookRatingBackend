using FBookRating.Models.DTOs.Category;
using FBookRating.Models.Entities;

namespace FBookRating.Services.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryReadDTO>> GetAllCategoriesAsync();
        Task<CategoryReadDTO> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(CategoryCreateDTO categoryCreateDTO);
        Task UpdateCategoryAsync(int id, CategoryUpdateDTO categoryUpdateDTO);
        Task DeleteCategoryAsync(int id);
    }
}
