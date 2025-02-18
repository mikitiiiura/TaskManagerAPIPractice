using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPIPractice.Contracts.Request
{
    public record ProjectRequest
    (
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        string Title,

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        string Description,

        DateTime? EndDate,

        [Required(ErrorMessage = "Status is required.")]
        [Range(0, 3, ErrorMessage = "Status must be between 0 and 3.")]
        int Status,

        Guid? TeamId
    );
}
