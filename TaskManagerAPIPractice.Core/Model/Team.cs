namespace TaskManagerAPIPractice.Core.Model
{
    public class Team
    {
        private Team(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public static (Team Team, string Error) Create(Guid id, string name)
        {
            var error = string.Empty;
            if (string.IsNullOrWhiteSpace(name))
                error += "Team name cannot be empty.\n";

            var team = new Team(id, name);
            return (team, error);
        }
    }
}
