﻿using Microsoft.AspNetCore.Mvc;
using TaskManagerAPIPractice.DataAccess.Repositories;
using TaskManagerAPIPractice.DataAccess.ModulEntity;
using TaskManagerAPIPractice.Core.Model;
using TaskManagerAPIPractice.Contracts;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Security.Claims;

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

            var project = await _projectsServices.GetAllByUser(Guid.Parse(userId));
            if (project == null) return NotFound();
            //return Ok(new ProjectResponse(project));
            return Ok(project.Select(p => new ProjectResponse(p)));
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

            var project = new ProjectEntity
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                StartDate = DateTime.UtcNow,
                EndDate = request.EndDate,
                Status = (ProjectStatus)request.Status,
                TeamId = request.TeamId,
                ProjectCreatedById = Guid.Parse(userId) //request.ProjectCreatedById
            };

            await _projectsServices.Add(project);
            return CreatedAtAction(nameof(GetById), new { id = project.Id }, new ProjectResponse(project));
        }

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] ProjectRequest request)
        //{
        //    var project = new ProjectEntity
        //    {
        //        Id = Guid.NewGuid(),
        //        Title = request.Title,
        //        Description = request.Description,
        //        StartDate = DateTime.UtcNow,
        //        EndDate = request.EndDate,
        //        Status = (ProjectStatus)request.Status,
        //        TeamId = request.TeamId,
        //        ProjectCreatedById = request.ProjectCreatedById
        //    };

        //    await _projectsServices.Add(project);
        //    return CreatedAtAction(nameof(GetById), new { id = project.Id }, new ProjectResponse(project));
        //}

        //Оновлення проекту ✅
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
