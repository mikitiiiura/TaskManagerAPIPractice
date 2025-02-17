namespace TaskManagerAPIPractice.Contracts.Request
{
    public record UserRequest(
        string? FullName,
        string? Email,
        Guid? TeamId,
        List<Guid>? Tags,
        List<Guid>? Tasks
    );
}
