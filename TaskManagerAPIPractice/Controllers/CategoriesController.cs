using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPIPractice.Application.Services;
using TaskManagerAPIPractice.Contracts;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryServices _categoryServices;

        public CategoriesController(ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoryResponse>>> GetAll()
        {
            var categories = await _categoryServices.GetAll();
            return Ok(categories.Select(c => new CategoryResponse(c)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponse>> GetById(Guid id)
        {
            var category = await _categoryServices.GetById(id);
            if (category == null) return NotFound();
            return Ok(new CategoryResponse(category));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryRequest request)
        {
            var category = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                CategoryCreatedById = request.CategoryCreatedById
            };

            await _categoryServices.Add(category);
            
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, new CategoryResponse(category));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CategoryRequest request)
        {
            var existingCategory = await _categoryServices.GetById(id);
            if (existingCategory == null) return NotFound();

            existingCategory.Title = request.Title;
            existingCategory.CategoryCreatedById = request.CategoryCreatedById;

            await _categoryServices.Update(existingCategory);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _categoryServices.Delete(id);
            return NoContent();
        }
    }
}
