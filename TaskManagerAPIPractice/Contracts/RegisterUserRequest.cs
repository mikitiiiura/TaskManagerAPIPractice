using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPIPractice.Contracts
{
    public record RegisterUserRequest
    (
        [Required] string UserName,
        [Required] string Password,
        [Required] string Email
    );
}
