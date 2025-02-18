using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPIPractice.Contracts.Request
{
    public record RegisterUserRequest
    (
         [Required(ErrorMessage = "Username is required.")] string UserName,

         [Required(ErrorMessage = "Email is required.")]
         [EmailAddress(ErrorMessage = "A valid email is required.")]
         string Email,

         [Required(ErrorMessage = "Password is required.")]
         [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
         string Password
    );
}
