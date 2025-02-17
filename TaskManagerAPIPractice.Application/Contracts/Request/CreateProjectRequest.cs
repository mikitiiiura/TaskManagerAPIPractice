namespace TaskManagerAPIPractice.Contracts.Request
{
    public record CreateProjectRequest(string Title, string Description, DateTime? EndDate, int Status, Guid? TeamId);
}
