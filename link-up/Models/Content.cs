namespace link_up.Models
{
    public class Content
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string content_id { get; set; }
        public string UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}