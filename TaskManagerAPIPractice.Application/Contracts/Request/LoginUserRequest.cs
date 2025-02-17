using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPIPractice.Contracts.Request
{
    public record LoginUserRequest
    (
        [Required] string Email,
        [Required] string Password
    );
}
