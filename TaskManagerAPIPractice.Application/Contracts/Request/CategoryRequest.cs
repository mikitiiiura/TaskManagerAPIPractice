using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPIPractice.Contracts.Request
{
    public record CategoryRequest
    (
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        string Title,

        [Required(ErrorMessage = "CategoryCreatedById is required.")]
        Guid CategoryCreatedById
    );
}
