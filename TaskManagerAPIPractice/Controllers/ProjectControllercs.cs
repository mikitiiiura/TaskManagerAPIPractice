using Microsoft.AspNetCore.Mvc;
using TaskManagerAPIPractice.DataAccess.Repositories;
using TaskManagerAPIPractice.DataAccess.ModulEntity;
using TaskManagerAPIPractice.Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IProjectsServices projectsServices, IMediator mediator, ILogger<ProjectsController> logger)
        {
            _projectsServices = projectsServices;
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectResponse>>> GetAll()
        {
            _logger.LogInformation("Fetching all projects");
            var projects = await _projectsServices.GetAll();
            return Ok(projects.Select(p => new ProjectResponse(p)));
        }

        [HttpGet("idUser")]
        public async Task<ActionResult<List<ProjectResponse>>> GetByIdAutomaticProject()
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized access attempt to GetByIdAutomaticProject.");
                return Unauthorized();
            }

            _logger.LogInformation("Fetching projects for user {UserId}", userId);
            var response = await _mediator.Send(new GetUserProjectQuery(Guid.Parse(userId)));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponse>> GetById(Guid id)
        {
            _logger.LogInformation("Fetching project with ID {ProjectId}", id);
            var project = await _projectsServices.GetById(id);
            if (project == null)
            {
                _logger.LogWarning("Project with ID {ProjectId} not found", id);
                return NotFound();
            }
            return Ok(new ProjectResponse(project));
        }

        [HttpPost("Automatically")]
        public async Task<IActionResult> CreateAutomaticallyUser([FromBody] CreateProjectRequest request)
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized access attempt to CreateAutomaticallyUser.");
                return Unauthorized();
            }

            _logger.LogInformation("Creating a new project for user {UserId}", userId);
            var command = new CreateProjectCommand(
                request.Title,
                request.Description,
                request.EndDate,
                request.Status,
                request.TeamId,
                Guid.Parse(userId)
            );

            var createdProject = await _mediator.Send(command);
            _logger.LogInformation("Project {ProjectId} created successfully", createdProject.Id);
            return CreatedAtAction(nameof(GetById), new { id = createdProject.Id }, createdProject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProjectRequest request)
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized access attempt to Update project {ProjectId}", id);
                return Unauthorized();
            }

            _logger.LogInformation("Updating project {ProjectId}", id);
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
            _logger.LogInformation("Project {ProjectId} updated successfully", id);
            return Ok(updatedProject);
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
        {
            _logger.LogInformation("Updating status of project {ProjectId}", id);
            await _projectsServices.UpdateStatus(id, (TaskManagerAPIPractice.Core.Model.ProjectStatus)request.Status);
            _logger.LogInformation("Status of project {ProjectId} updated successfully", id);
            return Ok(id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Deleting project {ProjectId}", id);
            await _projectsServices.Delete(id);
            _logger.LogInformation("Project {ProjectId} deleted successfully", id);
            return NoContent();
        }

        [HttpGet("filtered")]
        public async Task<ActionResult<List<ProjectResponse>>> GetFiltered([FromQuery] string? search, [FromQuery] int? status, [FromQuery] string? team)
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized access attempt to GetFiltered projects.");
                return Unauthorized();
            }

            _logger.LogInformation("Fetching filtered projects for user {UserId}", userId);
            var projects = await _projectsServices.GetFilteredProject(Guid.Parse(userId), search, status, team);
            var response = projects.Select(project => new ProjectResponse(project)).ToList();
            return Ok(response);
        }
    }
}
