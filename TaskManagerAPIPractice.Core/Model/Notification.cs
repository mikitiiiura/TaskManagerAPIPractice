namespace TaskManagerAPIPractice.Core.Model
{
    public class Notification
    {
        public Notification(Guid id, string message, bool isRead, DateTime createdAt)
        {
            Id = id;
            Message = message;
            IsRead = isRead;
            CreatedAt = createdAt;
        }
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }   //DateTime.UtcNow

        public static (Notification Notification, string Error) Create(Guid id, string message, bool isRead, DateTime createdAt)
        {
            var error = string.Empty;
            if (string.IsNullOrWhiteSpace(message))
                error += "Message cannot be empty.\n";

            var notification = new Notification(id, message, isRead, createdAt);
            return (notification, error);
        }
    }
}
