namespace TaskManagerAPIPractice.Contracts.Response
{
    public record UserResponse(
        Guid Id,
        string FullName,
        string Email,
        DateTime CreatedAt,
        TeamResp? Team,
        List<TagResp>? Tags,
        int TasksCount
    );
}
