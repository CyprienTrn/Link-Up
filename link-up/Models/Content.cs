using System.Text.Json.Serialization;

namespace link_up.Models
{
    public class Content
    {
        [JsonPropertyName("id")] // Mappe "Id" au format attendu "id"
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}