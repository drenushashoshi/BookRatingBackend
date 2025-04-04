using FBookRating.DataAccess.UnitOfWork;
using FBookRating.Models.DTOs.Category;
using FBookRating.Models.Entities;
using FBookRating.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace FBookRating.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get all categories.
        /// </summary>
        public async Task<IEnumerable<CategoryReadDTO>> GetAllCategoriesAsync()
        {
            var categories = await _unitOfWork.Repository<Category>().GetAll().ToListAsync();
            return categories.Select(c => new CategoryReadDTO
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            });
        }

        /// <summary>
        /// Get a category by ID.
        /// </summary>
        public async Task<CategoryReadDTO> GetCategoryByIdAsync(Guid id)
        {
            var category = await _unitOfWork.Repository<Category>().GetByCondition(c => c.Id == id).FirstOrDefaultAsync();
            if (category == null) return null;

            return new CategoryReadDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        /// <summary>
        /// Add a new category.
        /// </summary>
        public async Task AddCategoryAsync(CategoryCreateDTO categoryCreateDTO)
        {
            var category = new Category
            {
                Name = categoryCreateDTO.Name,
                Description = categoryCreateDTO.Description
            };

            _unitOfWork.Repository<Category>().Create(category);
            await _unitOfWork.Repository<Category>().SaveChangesAsync();
        }

        /// <summary>
        /// Update an existing category.
        /// </summary>
        /// <param name="id">ID of the category to update.</param>
        /// <param name="category">Updated category details.</param>
        public async Task UpdateCategoryAsync(Guid id, CategoryUpdateDTO categoryUpdateDTO)
        {
            var existingCategory = await _unitOfWork.Repository<Category>().GetByCondition(c => c.Id == id).FirstOrDefaultAsync();
            if (existingCategory == null) throw new Exception("Category not found.");

            existingCategory.Name = categoryUpdateDTO.Name;
            existingCategory.Description = categoryUpdateDTO.Description;

            _unitOfWork.Repository<Category>().Update(existingCategory);
            await _unitOfWork.Repository<Category>().SaveChangesAsync();
        }

        /// <summary>
        /// Delete a category.
        /// </summary>
        public async Task DeleteCategoryAsync(Guid id)
        {
            var category = await _unitOfWork.Repository<Category>().GetByCondition(c => c.Id == id).FirstOrDefaultAsync();
            if (category != null)
            {
                _unitOfWork.Repository<Category>().Delete(category);
                await _unitOfWork.Repository<Category>().SaveChangesAsync();
            }
        }
    }
}
