using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPIPractice.Contracts.Request
{
    public record UserRequest(
        [StringLength(100, ErrorMessage = "FullName cannot be longer than 100 characters.")]
        string? FullName,

        [EmailAddress(ErrorMessage = "A valid email is required.")]
        string? Email,
        Guid? TeamId,
        List<Guid>? Tags,
        List<Guid>? Tasks
    );
}
