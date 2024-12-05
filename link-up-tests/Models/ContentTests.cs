using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;
using link_up.Models;

namespace link_up_tests.Models
{
    public class ContentTests
    {
        [Fact]
        public void Content_ShouldInitializeWithDefaults()
        {
            // Arrange & Act
            var content = new Content();

            // Assert
            Assert.NotNull(content.id);
            Assert.NotEmpty(content.id);
            Assert.Null(content.content_id);
            Assert.Null(content.medias);
            Assert.Null(content.UserId);
            Assert.Equal(string.Empty, content.Title);
            Assert.Equal(string.Empty, content.Description);
            Assert.Equal(default(DateTime), content.CreatedAt);
            Assert.Equal(default(DateTime), content.UpdatedAt);
        }

        [Fact]
        public void Content_ShouldHandleMediasProperly()
        {
            // Arrange
            var media1 = new Media { MediaUrl = "http://example.com/image1.jpg", MediaType = "image" };
            var media2 = new Media { MediaUrl = "http://example.com/video1.mp4", MediaType = "video" };
            var medias = new List<Media> { media1, media2 };
            
            var content = new Content
            {
                medias = medias,
                UserId = "user123",
                Title = "Sample Title",
                Description = "Sample Description"
            };

            // Assert
            Assert.NotNull(content.medias);
            Assert.Equal(2, content.medias.Count);
            Assert.Equal("http://example.com/image1.jpg", content.medias[0].MediaUrl);
            Assert.Equal("video", content.medias[1].MediaType);
            Assert.Equal("user123", content.UserId);
        }

        [Fact]
        public void Content_ShouldSerializeToJsonIgnoringCertainProperties()
        {
            // Arrange
            var content = new Content
            {
                UserId = "user123",
                Title = "Sample Title",
                Description = "Sample Description",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var json = JsonSerializer.Serialize(content);
            var deserializedContent = JsonSerializer.Deserialize<Content>(json);

            // Assert
            Assert.Contains("Sample Title", json);
            Assert.DoesNotContain("CreatedAt", json);
            Assert.DoesNotContain("UpdatedAt", json);
            Assert.Equal("Sample Title", deserializedContent?.Title);
        }
    }
}
