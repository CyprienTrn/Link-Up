namespace link_up.Models
{
    public class Media
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string ContentId { get; set; } = string.Empty;
        public string MediaUrl { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}