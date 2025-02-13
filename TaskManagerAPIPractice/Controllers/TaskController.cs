using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagerAPIPractice.Application.Services;
using TaskManagerAPIPractice.Contracts;
using TaskManagerAPIPractice.DataAccess;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITasksService _tasksService;
        private readonly TaskAPIDbContext _dbContext;

        public TasksController(ITasksService tasksService, TaskAPIDbContext dbContext)
        {
            _tasksService = tasksService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<TaskResponse>>> GetAll()
        {
            var tasks = await _tasksService.GetAll();
            var response = tasks.Select(task => new TaskResponse(task)).ToList();
            return Ok(response);
        }

        [HttpGet("idUser")]
        public async Task<ActionResult<List<ProjectResponse>>> GetByIdAutomaticTask()
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null) return Unauthorized();

            var task = await _tasksService.GetAllByUser(Guid.Parse(userId));
            if (task == null) return NotFound();
            //return Ok(new ProjectResponse(project));
            return Ok(task.Select(t => new TaskResponse(t)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponse>> GetById(Guid id)
        {
            var task = await _tasksService.GetById(id);
            if (task == null)
                return NotFound();
            return Ok(new TaskResponse(task));
        }

        [HttpPost]
        public async Task<ActionResult<TaskResponse>> Create([FromBody] TaskAddRequest request)
        {
            var existingTags = _dbContext.Tags.Where(tag => request.Tags.Contains(tag.Id)).ToList();

            var taskEntity = new TaskEntity
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Description = request.Description,
                Status = (TaskManagerAPIPractice.Core.Model.TaskStatus)request.Status,
                Priority = (TaskManagerAPIPractice.Core.Model.TaskPriority)request.Priority,
                DeadLine = request.DeadLine,
                CreatedAt = DateTime.UtcNow,
                TaskCreatedById = request.TaskCreatedById,
                TaskAssignedToId = request.TaskAssignedToId,
                CategoryId = request.CategoryId,
                ProjectId = request.ProjectId,
                TeamId = request.TeamId,
                Tags = request.Tags?
                    .Select(tagId => existingTags.FirstOrDefault(t => t.Id == tagId) ?? new TagEntity { Id = tagId })
                    .ToList() ?? new List<TagEntity>()
            };

            await _tasksService.AddTaskWithNotification(taskEntity);

            return CreatedAtAction(nameof(GetById), new { id = taskEntity.Id }, new TaskResponse(taskEntity));
        }


        //[HttpPost]
        //public async Task<ActionResult<TaskResponse>> Create([FromBody] TaskAddRequest request)
        //{
        //    var taskEntity = new TaskEntity
        //    {
        //        Id = Guid.NewGuid(),
        //        Title = request.Title,
        //        Description = request.Description,
        //        Status = (TaskManagerAPIPractice.Core.Model.TaskStatus)request.Status,
        //        Priority = (TaskManagerAPIPractice.Core.Model.TaskPriority)request.Priority,
        //        DeadLine = request.DeadLine,
        //        CreatedAt = DateTime.UtcNow,
        //        TaskCreatedById = request.TaskCreatedById,
        //        TaskAssignedToId = request.TaskAssignedToId,
        //        CategoryId = request.CategoryId,
        //        ProjectId = request.ProjectId,
        //        TeamId = request.TeamId,
        //        Tags = request.Tags?.Select(tagId => _dbContext.Tags.Find(tagId) ?? new TagEntity { Id = tagId }).ToList()
        //        ?? new List<TagEntity>(),
        //        Notifications = new List<NotificationEntity>() // Порожній список, бо наповнюється в сервісі
        //    };

        //    await _tasksService.AddTaskWithNotification(taskEntity);

        //    return CreatedAtAction(nameof(GetById), new { id = taskEntity.Id }, new TaskResponse(taskEntity));
        //}



        //[HttpPost]
        //public async Task<ActionResult> Add([FromBody] TaskRequest taskRequest)
        //{

        //    var task = new TaskEntity
        //    {
        //        Id = Guid.NewGuid(),
        //        Title = taskRequest.Title,
        //        Description = taskRequest.Description,
        //        Status = (TaskManagerAPIPractice.Core.Model.TaskStatus)taskRequest.Status,
        //        Priority = (TaskManagerAPIPractice.Core.Model.TaskPriority)taskRequest.Priority,
        //        DeadLine = taskRequest.DeadLine,
        //        CreatedAt = DateTime.UtcNow,
        //        TaskCreatedById = taskRequest.TaskCreatedById,
        //        TaskAssignedToId = taskRequest.TaskAssignedToId,
        //        CategoryId = taskRequest.CategoryId,
        //        ProjectId = taskRequest.ProjectId,
        //        TeamId = taskRequest.TeamId,
        //        Tags = taskRequest.Tags?.Select(tagId => new TagEntity { Id = tagId }).ToList() ?? new List<TagEntity>(),
        //        Notifications = taskRequest.Notifications?.Select(n => new NotificationEntity { Id = n }).ToList() ?? new List<NotificationEntity>()
        //    };

        //    await _tasksService.Add(task);
        //    return CreatedAtAction(nameof(GetById), new { id = task.Id }, new TaskResponse(task));
        //}

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] TaskRequest taskRequest)
        {
            var existingTask = await _tasksService.GetById(id);
            if (existingTask == null)
                return NotFound();

            existingTask.Title = taskRequest.Title;
            existingTask.Description = taskRequest.Description;
            existingTask.Status = (TaskManagerAPIPractice.Core.Model.TaskStatus)taskRequest.Status;
            existingTask.Priority = (TaskManagerAPIPractice.Core.Model.TaskPriority)taskRequest.Priority;
            existingTask.DeadLine = taskRequest.DeadLine;
            existingTask.TaskAssignedToId = taskRequest.TaskAssignedToId;
            existingTask.CategoryId = taskRequest.CategoryId;
            existingTask.ProjectId = taskRequest.ProjectId;
            existingTask.TeamId = taskRequest.TeamId;
            existingTask.Tags = taskRequest.Tags?.Select(tagId => new TagEntity { Id = tagId }).ToList() ?? new List<TagEntity>();
            existingTask.Notifications = taskRequest.Notifications?.Select(n => new NotificationEntity { Id = n }).ToList() ?? new List<NotificationEntity>();

            await _tasksService.Update(existingTask);
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusRequest request)
        {
            await _tasksService.UpdateStatus(id, (TaskManagerAPIPractice.Core.Model.TaskStatus)request.Status);
            return Ok(id);
        }

        [HttpPatch("{id}/priority")]
        public async Task<ActionResult> UpdatePriority(Guid id, [FromBody] UpdatePriorityRequest request)
        {
            await _tasksService.UpdatePriority(id, (TaskManagerAPIPractice.Core.Model.TaskPriority)request.Priority);
            return Ok(id);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _tasksService.Delete(id);
            return NoContent();
        }

        [HttpGet("filtered")]
        public async Task<ActionResult<List<TaskResponse>>> GetFilteredTasks(
            [FromQuery] string? search,
            [FromQuery] int? status,
            [FromQuery] int? priority,
            [FromQuery] DateTime? deadline,
            [FromQuery] string? project,
            [FromQuery] string? tag)
        {
            var tasks = await _tasksService.GetFilteredTasks(search, status, priority, deadline, project, tag);
            var response = tasks.Select(task => new TaskResponse(task)).ToList();
            return Ok(response);
        }
    }
}

