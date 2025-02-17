using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPIPractice.Application.Services;
using TaskManagerAPIPractice.Contracts;
using TaskManagerAPIPractice.Contracts.Request;
using TaskManagerAPIPractice.Contracts.Response;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TagController : ControllerBase
    {
        private readonly ITagService _tagService;
        private readonly IUserService _userService;

        public TagController(ITagService tagService, IUserService userService)
        {
            _tagService = tagService;
            _userService = userService;
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TagResponse>>> GetAll()
        //{
        //    var tags = await _tagService.GetAllAsync();
        //    return Ok(tags);
        //}
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagResponse>>> GetAll()
        {
            var tags = await _tagService.GetAllAsync();
            var response = tags.Select(tag =>
                new TagResponse(
                    tag.Id,
                    tag.Name,
                    tag.TagCreatedBy != null ? new UserDetails(tag.TagCreatedBy.Id, tag.TagCreatedBy.FullName) : null,
                    tag.Tasks.Count // Тут буде правильний підрахунок
                )
            );
            return Ok(response);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TagResponse>> GetById(Guid id)
        {
            var tag = await _tagService.GetByIdAsync(id);
            if (tag == null) return NotFound();
            var taskCount = tag.Tasks.Count;
            return Ok(new TagResponse(tag.Id, tag.Name, tag.TagCreatedBy != null ? new UserDetails(tag.TagCreatedBy.Id, tag.TagCreatedBy.FullName) : null, taskCount));
        }

        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TagResponse>>> GetAll()
        //{
        //    var tags = await _tagService.GetAllAsync();
        //    var response = tags.Select(tag => new TagResponse(tag.Id, tag.Name, tag.TagCreatedBy != null ? new UserDetails(tag.TagCreatedBy.Id, tag.TagCreatedBy.FullName) : null));
        //    return Ok(response);
        //}


        //[HttpGet("{id}")]
        //public async Task<ActionResult<TagResponse>> GetById(Guid id)
        //{
        //    var tag = await _tagService.GetByIdAsync(id);
        //    if (tag == null) return NotFound();
        //    return Ok(tag);
        //}

        [HttpPost]
        public async Task<ActionResult<TagResponse>> Create([FromBody] TagRequest request)
        {
            var tag = new TagEntity
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                TagCreatedById = request.TagCreatedById
            };

            await _tagService.AddAsync(tag);

            // Отримуємо інформацію про користувача, якщо він є
            UserDetails? createdBy = null;
            if (request.TagCreatedById.HasValue)
            {
                var user = await _tagService.GetByIdAsync(request.TagCreatedById.Value);
                if (user != null)
                {
                    createdBy = new UserDetails(user.Id, user.Name);
                }
            }

            return CreatedAtAction(nameof(GetById), new { id = tag.Id }, new TagResponse(tag.Id, tag.Name, createdBy, 0));
        }
    //[HttpPost]
    //public async Task<ActionResult<TagResponse>> Create([FromBody] TagRequest request)
    //{
    //    var tag = new TagEntity
    //    {
    //        Id = Guid.NewGuid(),
    //        Name = request.Name,
    //        TagCreatedById = request.TagCreatedById
    //    };

    //    await _tagService.AddAsync(tag);

    //    // Отримуємо інформацію про користувача, якщо він є
    //    UserDetails? createdBy = null;
    //    if (request.TagCreatedById.HasValue)
    //    {
    //        //var user = await _tagService.GetByIdAsync(request.TagCreatedById.Value);
    //        var user = await _userService.GetById(request.TagCreatedById.Value);
    //        if (user != null)
    //        {
    //            createdBy = new UserDetails(user.Id, user.Name);
    //        }
    //    }

    //    return CreatedAtAction(nameof(GetById), new { id = tag.Id }, new TagResponse(tag.Id, tag.Name, createdBy, 0));
    //}


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] TagRequest request)
        {
            var existingTag = await _tagService.GetByIdAsync(id);
            if (existingTag == null) return NotFound();

            existingTag.Name = request.Name;
            existingTag.TagCreatedById = request.TagCreatedById;

            await _tagService.UpdateAsync(existingTag);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _tagService.DeleteAsync(id);
            return NoContent();
        }
    }
}
