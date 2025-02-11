namespace TaskManagerAPIPractice.Core.Model
{
    public class Category
    {
        private Category(Guid id, string title)
        {
            Id = id;
            Title = title;
        }
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public static (Category Category, string Error) Create(Guid id, string title)
        {
            var error = string.Empty;
            if (string.IsNullOrWhiteSpace(title))
                error += "Category title cannot be empty.\n";

            var category = new Category(id, title);
            return (category, error);
        }
    }
}
