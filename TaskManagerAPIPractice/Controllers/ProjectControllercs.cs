using Microsoft.AspNetCore.Mvc;
using TaskManagerAPIPractice.DataAccess.Repositories;
using TaskManagerAPIPractice.DataAccess.ModulEntity;
using TaskManagerAPIPractice.Core.Model;
using TaskManagerAPIPractice.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace TaskManagerAPIPractice.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectsServices _projectsServices;

        public ProjectsController(IProjectsServices projectsServices)
        {
            _projectsServices = projectsServices;
        }

        [HttpGet]
        public async Task<ActionResult<List<ProjectResponse>>> GetAll()
        {
            var projects = await _projectsServices.GetAll();
            return Ok(projects.Select(p => new ProjectResponse(p)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponse>> GetById(Guid id)
        {
            var project = await _projectsServices.GetById(id);
            if (project == null) return NotFound();
            return Ok(new ProjectResponse(project));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectRequest request)
        {
            var project = new ProjectEntity
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                StartDate = DateTime.UtcNow,
                EndDate = request.EndDate,
                Status = (ProjectStatus)request.Status,
                TeamId = request.TeamId,
                ProjectCreatedById = request.ProjectCreatedById
            };

            await _projectsServices.Add(project);
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, new ProjectResponse(project));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ProjectRequest request)
        {
            var existingProject = await _projectsServices.GetById(id);
            if (existingProject == null) return NotFound();

            existingProject.Title = request.Title;
            existingProject.Description = request.Description;
            existingProject.EndDate = request.EndDate;
            existingProject.Status = (ProjectStatus)request.Status;
            existingProject.TeamId = request.TeamId;
            existingProject.ProjectCreatedById = request.ProjectCreatedById;

            await _projectsServices.Update(existingProject);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _projectsServices.Delete(id);
            return NoContent();
        }

        //Отримати відфільтровані проекти
        [HttpGet("filtered")]
        public async Task<ActionResult<List<ProjectResponse>>> GetFiltered([FromQuery] string? search, [FromQuery] int? status, [FromQuery] string? team)
        {
            var projects = await _projectsServices.GetFilteredProject(search, status, team);
            var response = projects.Select(project => new ProjectResponse(project)).ToList();
            return Ok(response);
        }
    }
}





//using Microsoft.AspNetCore.Mvc;
//using TaskManagerAPI2.Contracts;
//using TaskManagerAPI2.Core.Model;
//using TaskManagerAPI2.Application.Services;

//namespace TaskManagerAPI2.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]

//    public class ProjectController : ControllerBase
//    {
//        private readonly IProjectsService _projectsService;

//        public ProjectController(IProjectsService projectsService)
//        {
//            _projectsService = projectsService;
//        }

//        // Отримати всі проекти
//        [HttpGet]
//        public async Task<ActionResult<List<ProjectResponse>>> GetAll()
//        {
//            var projects = await _projectsService.GetAllProject();
//            var response = projects.Select(p => new ProjectResponse(
//                p.Id, p.Title, p.Description, p.StartDate, p.EndDate, p.Status.ToString(), p.TeamId, p.ProjectCreatedById
//            ));
//            return Ok(response);
//        }

//        // Отримати проект за ID
//        [HttpGet("{id}")]
//        public async Task<ActionResult<ProjectResponse>> GetById(Guid id)
//        {
//            var project = await _projectsService.GetProjectById(id);
//            if (project == null) return NotFound("Project not found");

//            return Ok(new ProjectResponse(
//                project.Id, project.Title, project.Description, project.StartDate, project.EndDate, project.Status.ToString(), project.TeamId, project.ProjectCreatedById
//            ));
//        }

//        // Створити новий проект
//        [HttpPost]
//        public async Task<ActionResult<Guid>> Create([FromBody] ProjectRequest request)
//        {
//            if (!Enum.IsDefined(typeof(ProjectStatus), request.Status))
//                return BadRequest("Invalid status value");

//            var (project, error) = Project.Create(
//                Guid.NewGuid(), request.Title, request.Description, request.StartDate, request.EndDate, (ProjectStatus)request.Status
//            );

//            if (!string.IsNullOrEmpty(error))
//                return BadRequest(error);

//            project.TeamId = request.TeamId;
//            project.ProjectCreatedById = request.ProjectCreatedById;

//            await _projectsService.AddProject(project);
//            return CreatedAtAction(nameof(GetById), new { id = project.Id }, project.Id);
//        }

//        // Оновити проект
//        [HttpPut("{id}")]
//        public async Task<ActionResult> Update(Guid id, [FromBody] ProjectRequest request)
//        {
//            if (!Enum.IsDefined(typeof(ProjectStatus), request.Status))
//                return BadRequest("Invalid status value");

//            var existingProject = await _projectsService.GetProjectById(id);
//            if (existingProject == null) return NotFound("Project not found");

//            var project = new Project(
//                id, request.Title, request.Description, existingProject.StartDate, request.EndDate, (ProjectStatus)request.Status
//            )
//            {
//                TeamId = request.TeamId,
//                ProjectCreatedById = request.ProjectCreatedById
//            };

//            await _projectsService.UpdateProject(project);
//            return NoContent();
//        }

//        // Видалити проект
//        [HttpDelete("{id}")]
//        public async Task<ActionResult> Delete(Guid id)
//        {
//            await _projectsService.DeleteProject(id);
//            return NoContent();
//        }

//        // Отримати відфільтровані проекти
//        [HttpGet("filtered")]
//        public async Task<ActionResult<List<ProjectResponse>>> GetFiltered([FromQuery] string? search, [FromQuery] int? status, [FromQuery] string? team)
//        {
//            if (status.HasValue && !Enum.IsDefined(typeof(ProjectStatus), status.Value))
//                return BadRequest("Invalid status value");

//            var projects = await _projectsService.GetFilteredProjects(search, status, team);
//            var response = projects.Select(p => new ProjectResponse(
//                p.Id, p.Title, p.Description, p.StartDate, p.EndDate, p.Status.ToString(), p.TeamId, p.ProjectCreatedById
//            ));
//            return Ok(response);
//        }
//    }
//}