namespace TaskManagerAPIPractice.Contracts.Request
{
    public record ProjectRequest(string Title, string Description, DateTime? EndDate, int Status, Guid? TeamId);
}
