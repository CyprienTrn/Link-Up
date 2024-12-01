using System.Text.Json.Serialization;

namespace link_up.Models
{
    public class Media
    {
        [JsonPropertyName("id")] // Mappe "Id" au format attendu "id"
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ContentId { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}