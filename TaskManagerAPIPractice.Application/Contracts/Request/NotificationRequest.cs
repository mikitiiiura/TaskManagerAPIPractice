namespace TaskManagerAPIPractice.Contracts.Request
{
    public record NotificationRequest(
        string Message,
        bool IsRead,
        Guid UserId,
        Guid? TaskId
    );
}
