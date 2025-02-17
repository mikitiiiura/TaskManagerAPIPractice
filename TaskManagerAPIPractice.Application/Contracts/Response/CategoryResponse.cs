using TaskManagerAPIPractice.DataAccess.ModulEntity;

namespace TaskManagerAPIPractice.Contracts.Response
{
    public record CategoryResponse
    (
        Guid Id,
        string Title,
        UserDetails CategoryCreatedBy,
        int TaskCount
    )
    {
        public CategoryResponse(CategoryEntity category) : this(
            category.Id,
            category.Title,
            new UserDetails(category.CategoryCreatedBy.Id, category.CategoryCreatedBy.FullName),
            category.Tasks.Count)
        { }
    }
}
