using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPIPractice.Contracts.Request
{
    public record UpdateStatusRequest
    (
        [Required(ErrorMessage = "Status is required.")]
        [Range(0, 3, ErrorMessage = "Status must be between 0 and 3.")]
        int Status
    );
}
