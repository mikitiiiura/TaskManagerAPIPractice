using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskManagerAPIPractice.Application.Services;
using TaskManagerAPIPractice.Contracts.Request;
using TaskManagerAPIPractice.Contracts.Response;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryServices _categoryServices;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryServices categoryServices, ILogger<CategoriesController> logger)
        {
            _categoryServices = categoryServices;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryResponse>>> GetAll()
        {
            _logger.LogInformation("Fetching all categories");
            var categories = await _categoryServices.GetAll();
            return Ok(categories.Select(c => new CategoryResponse(c)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponse>> GetById(Guid id)
        {
            _logger.LogInformation("Fetching category with ID: {CategoryId}", id);
            var category = await _categoryServices.GetById(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID: {CategoryId} not found", id);
                return NotFound();
            }
            return Ok(new CategoryResponse(category));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryRequest request)
        {
            _logger.LogInformation("Creating a new category with title: {Title}", request.Title);
            var category = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                CategoryCreatedById = request.CategoryCreatedById
            };

            await _categoryServices.Add(category);
            _logger.LogInformation("Category created with ID: {CategoryId}", category.Id);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, new CategoryResponse(category));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CategoryRequest request)
        {
            _logger.LogInformation("Updating category with ID: {CategoryId}", id);
            var existingCategory = await _categoryServices.GetById(id);
            if (existingCategory == null)
            {
                _logger.LogWarning("Category with ID: {CategoryId} not found for update", id);
                return NotFound();
            }

            existingCategory.Title = request.Title;
            existingCategory.CategoryCreatedById = request.CategoryCreatedById;

            await _categoryServices.Update(existingCategory);
            _logger.LogInformation("Category with ID: {CategoryId} updated successfully", id);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Deleting category with ID: {CategoryId}", id);
            await _categoryServices.Delete(id);
            _logger.LogInformation("Category with ID: {CategoryId} deleted successfully", id);
            return NoContent();
        }
    }
}