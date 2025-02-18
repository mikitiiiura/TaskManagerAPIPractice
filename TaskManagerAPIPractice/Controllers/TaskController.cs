using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using TaskManagerAPIPractice.Application.Contracts.Command;
using TaskManagerAPIPractice.Application.Handlers;
using TaskManagerAPIPractice.Application.Services;
using TaskManagerAPIPractice.Contracts;
using TaskManagerAPIPractice.Contracts.Request;
using TaskManagerAPIPractice.Contracts.Response;
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
        private readonly IMediator _mediator;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITasksService tasksService, TaskAPIDbContext dbContext, IMediator mediator, ILogger<TasksController> logger)
        {
            _tasksService = tasksService;
            _dbContext = dbContext;
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("idUser")]
        public async Task<ActionResult<List<TaskResponse>>> GetByIdAutomaticTask()
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized access attempt to GetByIdAutomaticTask.");
                return Unauthorized();
            }

            _logger.LogInformation("Fetching tasks for user {UserId}", userId);
            var response = await _mediator.Send(new GetUserTasksQuery(Guid.Parse(userId)));
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskResponse>> GetById(Guid id)
        {
            _logger.LogInformation("Fetching task with ID {TaskId}", id);
            var task = await _tasksService.GetById(id);
            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} not found", id);
                return NotFound();
            }
            return Ok(new TaskResponse(task));
        }

        [HttpPost]
        public async Task<ActionResult<TaskResponse>> Create([FromBody] TaskAddUpdateRequest request)
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized access attempt to Create task.");
                return Unauthorized();
            }

            _logger.LogInformation("Creating a new task for user {UserId}", userId);
            var command = new CreateTaskCommand(
                request.Title,
                request.Description,
                request.Status,
                request.Priority,
                request.DeadLine,
                request.TaskAssignedToId,
                request.CategoryId,
                request.ProjectId,
                request.Tags,
                request.TeamId,
                Guid.Parse(userId)
            );

            var createdTask = await _mediator.Send(command);
            _logger.LogInformation("Task {TaskId} created successfully", createdTask.Id);
            return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] TaskAddUpdateRequest request)
        {
            _logger.LogInformation("Updating task {TaskId}", id);
            var command = new UpdateTaskCommand(
                id,
                request.Title,
                request.Description,
                request.Status,
                request.Priority,
                request.DeadLine,
                request.TaskAssignedToId,
                request.CategoryId,
                request.ProjectId,
                request.Tags,
                request.TeamId
            );

            var updatedTask = await _mediator.Send(command);
            _logger.LogInformation("Task {TaskId} updated successfully", id);
            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Deleting task {TaskId}", id);
            await _tasksService.Delete(id);
            _logger.LogInformation("Task {TaskId} deleted successfully", id);
            return NoContent();
        }
    }
}
