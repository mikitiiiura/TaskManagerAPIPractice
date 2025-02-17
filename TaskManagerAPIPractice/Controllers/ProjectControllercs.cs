using Microsoft.AspNetCore.Mvc;
using TaskManagerAPIPractice.DataAccess.Repositories;
using TaskManagerAPIPractice.DataAccess.ModulEntity;
using TaskManagerAPIPractice.Core.Model;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;
using TaskManagerAPIPractice.Contracts.Request;
using TaskManagerAPIPractice.Contracts.Response;
using TaskManagerAPIPractice.Contracts;
using MediatR;
using TaskManagerAPIPractice.Application.Contracts.Command;

namespace TaskManagerAPIPractice.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsServices _projectsServices;
        private readonly IMediator _mediator;

        public ProjectsController(IProjectsServices projectsServices, IMediator mediator)
        {
            _projectsServices = projectsServices;
            _mediator = mediator;
        }

        //Отримання проектів ✅
        [HttpGet]
        public async Task<ActionResult<List<ProjectResponse>>> GetAll()
        {
            var projects = await _projectsServices.GetAll();
            return Ok(projects.Select(p => new ProjectResponse(p)));
        }

        //Отримання проектів  з автоматичним підставленням користувача ✅
        [HttpGet("idUser")]
        public async Task<ActionResult<List<ProjectResponse>>> GetByIdAutomaticProject()
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null) return Unauthorized();

            var response = await _mediator.Send(new GetUserProjectQuery(Guid.Parse(userId)));
            return Ok(response);
        }

        //Отримання проктів по ідентифікатору ✅
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponse>> GetById(Guid id)
        {
            var project = await _projectsServices.GetById(id);
            if (project == null) return NotFound();
            return Ok(new ProjectResponse(project));
        }


        //Створення проекту з автоматичним підставленням користувача ✅
        [HttpPost("Automatically")]
        public async Task<IActionResult> CreateAutomaticallyUser([FromBody] CreateProjectRequest request)
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null) return Unauthorized();

            var command = new CreateProjectCommand(
                request.Title,
                request.Description,
                request.EndDate,
                request.Status,
                request.TeamId,
                Guid.Parse(userId)
            );

            var createdProject = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetById), new { id = createdProject.Id }, createdProject);
        }

        //Оновлення проекту ✅
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProjectRequest request)
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null) return Unauthorized();

            var command = new UpdateProjectCommand(
                id,
                request.Title,
                request.Description,
                request.EndDate,
                request.Status,
                request.TeamId,
                Guid.Parse(userId)
            );

            var updatedProject = await _mediator.Send(command);

            return Ok(updatedProject);
        }

        //Оовлення статусу ✅
        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
        {
            await _projectsServices.UpdateStatus(id, (TaskManagerAPIPractice.Core.Model.ProjectStatus)request.Status);
            return Ok(id);
        }

        //Видалення проекту ✅
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _projectsServices.Delete(id);
            return NoContent();
        }

        //Отримати відфільтровані проекти✅
        [HttpGet("filtered")]
        public async Task<ActionResult<List<ProjectResponse>>> GetFiltered([FromQuery] string? search, [FromQuery] int? status, [FromQuery] string? team)
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null) return Unauthorized();

            var projects = await _projectsServices.GetFilteredProject(Guid.Parse(userId), search, status, team);
            var response = projects.Select(project => new ProjectResponse(project)).ToList();
            return Ok(response);
        }
    }
}
