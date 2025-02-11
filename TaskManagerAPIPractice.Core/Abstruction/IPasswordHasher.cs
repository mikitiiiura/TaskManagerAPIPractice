namespace TaskManagerAPIPractice.Persistence
{
    public interface IPasswordHasher
    {
        string Generate(string password);
        bool Verify(string password, string hasherPassword);
    }
}

