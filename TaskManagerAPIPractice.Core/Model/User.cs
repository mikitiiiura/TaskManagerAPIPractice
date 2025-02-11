
using System.Text.RegularExpressions;

namespace TaskManagerAPIPractice.Core.Model
{
    public class User
    {
        public const int MAX_TITLE_LENGTH = 255;
        public const int MAX_EMAIL_LENGTH = 500;

        public User(Guid id, string fullName, string email, string passwordHash, DateTime createdAt)
        {
            Id = id;
            FullName = fullName;
            Email = email;
            PasswordHash = passwordHash;
            CreatedAt = createdAt;
        }

        public Guid Id { get;}
        public string FullName { get;} = string.Empty;
        public string Email { get;} = string.Empty;
        public string PasswordHash { get; } = string.Empty;
        public DateTime CreatedAt { get; }

        public static (User User, string Error) Create(Guid id, string fullName, string email, string passwordHash, DateTime createdAt)
        {
            var error = string.Empty;

            if (string.IsNullOrWhiteSpace(fullName) || fullName.Length > MAX_TITLE_LENGTH)
            {
                error += "Full name cannot be empty or longer than 255 characters.\n";
            }

            if (string.IsNullOrWhiteSpace(email) || email.Length > MAX_EMAIL_LENGTH || !IsValidEmail(email))
            {
                error += "Invalid email format or email is too long.\n";
            }

            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                error += "Password hash cannot be empty.\n";
            }


            var user = new User(id, fullName, email, passwordHash, createdAt);
            return (user, error);
        }

        private static bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");
            return emailRegex.IsMatch(email);
        }
    }
}
