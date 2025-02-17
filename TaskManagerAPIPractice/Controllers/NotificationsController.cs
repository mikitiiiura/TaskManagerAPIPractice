using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskManagerAPIPractice.Application.Services;
using TaskManagerAPIPractice.Contracts;
using TaskManagerAPIPractice.Contracts.Request;
using TaskManagerAPIPractice.Contracts.Response;
using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationsService _notificationsService;

        public NotificationsController(INotificationsService notificationsService)
        {
            _notificationsService = notificationsService;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotificationResponse>>> GetAll()
        {
            var notifications = await _notificationsService.GetAll();
            return Ok(notifications.Select(n => new NotificationResponse(n.Id, n.Message, n.IsRead, n.CreatedAt, new UserDetails(n.User.Id, n.User.FullName), n.Task != null ? new TaskDetails(n.Task.Id, n.Task.Title) : null)));
        }


        [HttpGet("idUser")]
        public async Task<ActionResult<NotificationResponse>> GetNotificationByIdUser()
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null) return Unauthorized();

            var notifications = await _notificationsService.GetByIdUser(Guid.Parse(userId));
            if (notifications == null) return NotFound();
            //return Ok(new NotificationResponse(notification.Id, notification.Message, notification.IsRead, notification.CreatedAt, new UserDetails(notification.User.Id, notification.User.FullName), notification.Task != null ? new TaskDetails(notification.Task.Id, notification.Task.Title) : null));
            return Ok(notifications.Select(notification => new NotificationResponse(notification.Id, notification.Message, notification.IsRead, notification.CreatedAt, new UserDetails(notification.User.Id, notification.User.FullName), notification.Task != null ? new TaskDetails(notification.Task.Id, notification.Task.Title) : null)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationResponse>> GetById(Guid id)
        {
            var notification = await _notificationsService.GetById(id);
            if (notification == null) return NotFound();
            return Ok(new NotificationResponse(notification.Id, notification.Message, notification.IsRead, notification.CreatedAt, new UserDetails(notification.User.Id, notification.User.FullName), notification.Task != null ? new TaskDetails(notification.Task.Id, notification.Task.Title) : null));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationRequest request)
        {
            var notification = new NotificationEntity
            {
                Id = Guid.NewGuid(),
                Message = request.Message,
                IsRead = request.IsRead,
                CreatedAt = DateTime.UtcNow,
                UserId = request.UserId,
                TaskId = request.TaskId
            };

            await _notificationsService.Add(notification);
            return CreatedAtAction(nameof(GetById), new { id = notification.Id }, new NotificationResponse(notification.Id, notification.Message, notification.IsRead, notification.CreatedAt, new UserDetails(notification.User.Id, notification.User.FullName), notification.Task != null ? new TaskDetails(notification.Task.Id, notification.Task.Title) : null));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] NotificationRequest request)
        {
            var existingNotification = await _notificationsService.GetById(id);
            if (existingNotification == null) return NotFound();

            existingNotification.Message = request.Message;
            existingNotification.IsRead = request.IsRead;
            existingNotification.UserId = request.UserId;
            existingNotification.TaskId = request.TaskId;

            await _notificationsService.Update(existingNotification);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _notificationsService.Delete(id);
            return NoContent();
        }
    }
}