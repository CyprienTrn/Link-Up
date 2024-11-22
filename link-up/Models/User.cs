using System.Text.Json.Serialization;

namespace link_up.Models
{
    public class User
    {
        [JsonPropertyName("id")] // Mappe "Id" au format attendu "id"
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsPrivate { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}