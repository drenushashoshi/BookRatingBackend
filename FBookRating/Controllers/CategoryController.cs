using FBookRating.Models.DTOs.Category;
using FBookRating.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FBookRating.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryCreateDTO categoryCreateDTO)
        {
            await _categoryService.AddCategoryAsync(categoryCreateDTO);
            return Ok("Category created successfully.");
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] CategoryUpdateDTO categoryUpdateDTO)
        {
            await _categoryService.UpdateCategoryAsync(id, categoryUpdateDTO);
            return Ok("Category updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            return Ok("Category deleted successfully.");
        }
    }
}
