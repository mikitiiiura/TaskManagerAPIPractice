using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(INotificationsService notificationsService, ILogger<NotificationsController> logger)
        {
            _notificationsService = notificationsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<NotificationResponse>>> GetAll()
        {
            _logger.LogInformation("Fetching all notifications");
            var notifications = await _notificationsService.GetAll();
            return Ok(notifications.Select(n => new NotificationResponse(n.Id, n.Message, n.IsRead, n.CreatedAt, new UserDetails(n.User.Id, n.User.FullName), n.Task != null ? new TaskDetails(n.Task.Id, n.Task.Title) : null)));
        }

        [HttpGet("idUser")]
        public async Task<ActionResult<NotificationResponse>> GetNotificationByIdUser()
        {
            var userId = User.FindFirstValue("userId");
            if (userId == null)
            {
                _logger.LogWarning("Unauthorized access attempt to GetNotificationByIdUser");
                return Unauthorized();
            }

            _logger.LogInformation("Fetching notifications for user {UserId}", userId);
            var notifications = await _notificationsService.GetByIdUser(Guid.Parse(userId));
            if (notifications == null) return NotFound();
            return Ok(notifications.Select(notification => new NotificationResponse(notification.Id, notification.Message, notification.IsRead, notification.CreatedAt, new UserDetails(notification.User.Id, notification.User.FullName), notification.Task != null ? new TaskDetails(notification.Task.Id, notification.Task.Title) : null)));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationResponse>> GetById(Guid id)
        {
            _logger.LogInformation("Fetching notification by ID {NotificationId}", id);
            var notification = await _notificationsService.GetById(id);
            if (notification == null) return NotFound();
            return Ok(new NotificationResponse(notification.Id, notification.Message, notification.IsRead, notification.CreatedAt, new UserDetails(notification.User.Id, notification.User.FullName), notification.Task != null ? new TaskDetails(notification.Task.Id, notification.Task.Title) : null));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationRequest request)
        {
            _logger.LogInformation("Creating new notification for user {UserId}", request.UserId);
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
            _logger.LogInformation("Updating notification {NotificationId}", id);
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
            _logger.LogWarning("Deleting notification {NotificationId}", id);
            await _notificationsService.Delete(id);
            return NoContent();
        }
    }
}
