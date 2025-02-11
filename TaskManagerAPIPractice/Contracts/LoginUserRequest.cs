using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPIPractice.Contracts
{
    public record LoginUserRequest
    (
        [Required] string Email,
        [Required] string Password
    );
}
