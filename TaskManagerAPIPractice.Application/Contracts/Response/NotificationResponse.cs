namespace TaskManagerAPIPractice.Contracts.Response
{
    public record NotificationResponse(
        Guid Id,
        string Message,
        bool IsRead,
        DateTime CreatedAt,
        UserDetails User,
        TaskDetails? Task
    );
}
