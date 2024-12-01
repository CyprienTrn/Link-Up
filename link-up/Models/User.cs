using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Text;

namespace link_up.Models
{
    public class User
    {
        // [JsonPropertyName("id")] // Mappe "Id" au format attendu "id"
        public string Id { get; set; } = Guid.NewGuid().ToString();
        // [JsonProperty(PropertyName = "partitionKey")]
        // public string PartitionKey { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Id: {Id}");
            sb.AppendLine($"Email: {Email}");
            sb.AppendLine($"Password: {Password}");
            sb.AppendLine($"Name: {Name}");
            sb.AppendLine($"IsPrivate: {IsPrivate}");
            sb.AppendLine($"CreatedAt: {CreatedAt}");
            return sb.ToString();
        }
    }
}