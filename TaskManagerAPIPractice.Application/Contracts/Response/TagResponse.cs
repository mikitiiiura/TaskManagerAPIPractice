namespace TaskManagerAPIPractice.Contracts.Response
{
    public record TagResponse(Guid Id, string Name, UserDetails? CreatedBy, int TaskCount);
}
