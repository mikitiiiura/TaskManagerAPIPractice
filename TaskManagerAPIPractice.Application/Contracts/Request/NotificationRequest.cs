using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPIPractice.Contracts.Request
{
    public record NotificationRequest(
        [Required(ErrorMessage = "Message is required.")]
        [StringLength(500, ErrorMessage = "Message cannot be longer than 500 characters.")]
        string Message,
        bool IsRead,
        [Required(ErrorMessage = "UserId is required.")]
        Guid UserId,
        Guid? TaskId
    );
}
