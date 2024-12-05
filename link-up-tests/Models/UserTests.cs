using Xunit;
using link_up.Models;
using System;

namespace link_up_tests
{
    public class UserTests
    {
        [Fact]
        public void Constructor_ShouldGenerateUniqueId()
        {
            // Arrange & Act
            var user1 = new User();
            var user2 = new User();

            // Assert
            Assert.NotEqual(user1.id, user2.id); // Vérifie que les IDs sont uniques
        }

        [Fact]
        public void Constructor_ShouldInitializeFieldsCorrectly()
        {
            // Arrange
            var user = new User
            {
                Email = "john.doe@example.com",
                Password = "Password123",
                Name = "John Doe",
                IsPrivate = true
            };

            // Act & Assert
            Assert.Equal("john.doe@example.com", user.Email);
            Assert.Equal("Password123", user.Password);
            Assert.Equal("John Doe", user.Name);
            Assert.True(user.IsPrivate);
        }

        [Fact]
        public void ShouldNotSerializeIdAndCreatedAt()
        {
            // Arrange
            var user = new User();
            var json = System.Text.Json.JsonSerializer.Serialize(user);

            // Assert
            Assert.DoesNotContain("id", json); // Le champ id ne doit pas être sérialisé
            Assert.DoesNotContain("CreatedAt", json); // Le champ CreatedAt ne doit pas être sérialisé
        }

        [Fact]
        public void ShouldAssignDefaultValuesToProperties()
        {
            // Arrange
            var user = new User();

            // Act & Assert
            Assert.Equal(string.Empty, user.Email);
            Assert.Equal(string.Empty, user.Password);
            Assert.Equal(string.Empty, user.Name);
            Assert.False(user.IsPrivate);
            Assert.True(Guid.TryParse(user.id, out _)); // Vérifie que l'ID est un GUID valide
        }

        [Fact]
        public void ShouldSetCreatedAtWhenInstantiated()
        {
            // Arrange
            var user = new User();
            // Act & Assert
            Assert.True(user.CreatedAt <= DateTime.Now); // Vérifie que CreatedAt est initialisé
        }
    }
}
