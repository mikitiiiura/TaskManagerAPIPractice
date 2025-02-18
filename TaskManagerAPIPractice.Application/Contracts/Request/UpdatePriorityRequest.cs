using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPIPractice.Contracts.Request
{
    public record UpdatePriorityRequest
    (
        [Required(ErrorMessage = "Priority is required.")]
        [Range(0, 2, ErrorMessage = "Priority must be between 0 and 2.")]
        int Priority
    );
}
