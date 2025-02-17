namespace TaskManagerAPIPractice.Contracts.Request
{
    public record CategoryRequest
    (
        string Title,
        Guid CategoryCreatedById
    );
}
