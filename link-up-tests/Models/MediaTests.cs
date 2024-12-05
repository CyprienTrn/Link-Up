using System;
using System.Text.Json;
using Xunit;
using link_up.Models;

namespace link_up_tests.Models
{
    public class MediaTests
    {
        [Fact]
        public void Media_ShouldInitializeWithDefaults()
        {
            // Arrange & Act
            var media = new Media();

            // Assert
            Assert.NotNull(media.id);
            Assert.NotEmpty(media.id);
            Assert.Equal(string.Empty, media.media_id);
            Assert.Equal(string.Empty, media.ContentId);
            Assert.Equal(string.Empty, media.MediaUrl);
            Assert.Equal(string.Empty, media.MediaType);
            Assert.Equal(default(DateTime), media.UploadedAt);
        }

        [Fact]
        public void Media_ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var media = new Media
            {
                media_id = "media123",
                ContentId = "content123",
                MediaUrl = "http://example.com/image.jpg",
                MediaType = "image",
                UploadedAt = DateTime.UtcNow
            };

            // Act
            var result = media.ToString();

            // Assert
            Assert.Contains("media123", result);
            Assert.Contains("http://example.com/image.jpg", result);
            Assert.Contains("image", result);
        }

        [Fact]
        public void Media_ShouldSerializeToJsonIgnoringCertainProperties()
        {
            // Arrange
            var media = new Media
            {
                MediaUrl = "http://example.com/image.jpg",
                MediaType = "image",
                UploadedAt = DateTime.UtcNow
            };

            // Act
            var json = JsonSerializer.Serialize(media);
            var deserializedMedia = JsonSerializer.Deserialize<Media>(json);

            // Assert
            Assert.Contains("http://example.com/image.jpg", json);
            Assert.DoesNotContain("UploadedAt", json);
            Assert.Equal("http://example.com/image.jpg", deserializedMedia?.MediaUrl);
        }
    }
}
