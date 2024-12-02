using System.Text;

namespace link_up.Models
{
    public class Media
    {
        public string id { get; set; } = Guid.NewGuid().ToString();
        public string media_id { get; set; } = string.Empty;
        public string ContentId { get; set; } = string.Empty;
        public string MediaUrl { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Id: {id}");
            sb.AppendLine($"media_id: {media_id}");
            sb.AppendLine($"ContentId: {ContentId}");
            sb.AppendLine($"MediaUrl: {MediaUrl}");
            sb.AppendLine($"MediaType: {MediaType}");
            sb.AppendLine($"UploadedAt: {UploadedAt}");
            return sb.ToString();
        }
    }
}