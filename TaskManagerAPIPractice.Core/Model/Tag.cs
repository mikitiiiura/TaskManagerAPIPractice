namespace TaskManagerAPIPractice.Core.Model
{
    public class Tag
    {
        private Tag(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public static (Tag Tag, string Error) Create(Guid id, string name)
        {
            var error = string.Empty;
            if (string.IsNullOrWhiteSpace(name))
                error += "Tag name cannot be empty.\n";

            var tag = new Tag(id, name);
            return (tag, error);
        }
    }
}
