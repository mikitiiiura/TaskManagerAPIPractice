using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPIPractice.Contracts.Request
{
    public record LoginUserRequest
    (
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "A valid email is required.")]
        string Email,

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        string Password
    );
}
