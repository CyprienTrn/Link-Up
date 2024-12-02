using System.Text;

namespace link_up.Models
{
    public class User
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string user_id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Id: {id}");
            // sb.AppendLine($"userid: {user_id}");
            sb.AppendLine($"Email: {Email}");
            sb.AppendLine($"Password: {Password}");
            sb.AppendLine($"Name: {Name}");
            sb.AppendLine($"IsPrivate: {IsPrivate}");
            sb.AppendLine($"CreatedAt: {CreatedAt}");
            return sb.ToString();
        }
    }
}