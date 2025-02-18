using MediatR;
using System.ComponentModel.DataAnnotations;
using TaskManagerAPIPractice.Contracts.Response;

namespace TaskManagerAPIPractice.Contracts.Request
{
    public record TaskAddUpdateRequest(
    [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        string Title,

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        string Description,

        [Required(ErrorMessage = "Status is required.")]
        [Range(0, 3, ErrorMessage = "Status must be between 0 and 4.")]
        int Status,

        [Required(ErrorMessage = "Priority is required.")]
        [Range(0, 2, ErrorMessage = "Priority must be between 0 and 5.")]
        int Priority,

        DateTime? DeadLine,
        Guid? TaskAssignedToId,
        Guid? CategoryId,
        Guid? ProjectId,
        List<Guid> Tags,
        Guid? TeamId
    ) : IRequest<TaskResponse>;
}
