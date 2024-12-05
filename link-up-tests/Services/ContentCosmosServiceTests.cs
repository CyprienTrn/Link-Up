using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using link_up.Models;
using link_up.Services;
using Microsoft.Azure.Cosmos;


namespace link_up_tests.Services
{
    public class ContentCosmosServiceTests
    {
        private readonly Mock<UserCosmosService> _mockUserService;
        private readonly Mock<MediaCosmosService> _mockMediaService;
        private readonly Mock<Container> _mockContainer;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly ContentCosmosService _contentService;

        public ContentCosmosServiceTests()
        {
            // Mock de IConfiguration
            _mockConfiguration = new Mock<IConfiguration>();

            // Mock de la section "CosmosDbCyp" dans IConfiguration
            var cosmosSection = new Mock<IConfigurationSection>();
            cosmosSection.Setup(x => x["EndpointUri"]).Returns("https://link-up-test.documents.azure.com:443/");
            cosmosSection.Setup(x => x["PrimaryKey"]).Returns("6TvtA3Vo4EfqiFyrggZh6WwVHpq2zOrUNC9eBvgyJbm2PhdVyRqw990MgI423I5L8k7XbOkQCi5uACDb7cJQhw==");
            cosmosSection.Setup(x => x["DatabaseId"]).Returns("link-up_bdd");
            cosmosSection.Setup(x => x["ContainerContentId"]).Returns("content");
            cosmosSection.Setup(x => x["ContentPartitionKey"]).Returns("/content_id");

            cosmosSection.Setup(x => x["ContainerUserId"]).Returns("users");
            cosmosSection.Setup(x => x["UserPartitionKey"]).Returns("/user_id");

            cosmosSection.Setup(x => x["ContainerMediaId"]).Returns("media");
            cosmosSection.Setup(x => x["MediaPartitionKey"]).Returns("/media_id");

            _mockConfiguration.Setup(x => x.GetSection("CosmosDbCyp")).Returns(cosmosSection.Object);

            // Création des mocks pour les services UserCosmosService et MediaCosmosService
            _mockUserService = new Mock<UserCosmosService>(_mockConfiguration.Object);
            _mockMediaService = new Mock<MediaCosmosService>(_mockConfiguration.Object);

            // Création du mock de Container
            _mockContainer = new Mock<Container>();

            // Instanciation de ContentCosmosService avec les mocks
            _contentService = new ContentCosmosService(
                _mockConfiguration.Object,
                _mockUserService.Object,
                _mockMediaService.Object
            );
        }

        // [Fact]
        // public async Task CreateContentAsync_ShouldCreateContentSuccessfully()
        // {
        //     // Arrange
        //     var content = new Content
        //     {
        //         UserId = "user123",
        //         Title = "Test Title",
        //         Description = "Test Description",
        //         medias = new List<Media>
        //         {
        //             new Media { MediaUrl = "http://example.com/media1.jpg", MediaType = "image" }
        //         }
        //     };

        //     // Simulation de la vérification de l'utilisateur
        //     // _mockUserService.Setup(s => s.CheckUserExistsAsync("user123")).ReturnsAsync(true);

        //     // Simulation de la réponse ItemResponse<Content>
        //     var mockResponse = new Mock<ItemResponse<Content>>();
        //     mockResponse.SetupGet(r => r.Resource).Returns(content);

        //     // Setup du mock de la méthode CreateItemAsync pour ContentCosmosService
        //     _mockContainer
        //         .Setup(c => c.CreateItemAsync(It.IsAny<Content>(), It.IsAny<PartitionKey>(), null, default))
        //         .ReturnsAsync(mockResponse.Object);  // Retourne une Task<ItemResponse<Content>>

        //     // Act
        //     var result = await _contentService.CreateContentAsync(content);

        //     // Assert
        //     Assert.NotNull(result);
        //     Assert.NotEmpty(result.id);
        //     Assert.Equal("Test Title", result.Title);
        // }

        [Fact]
        public async Task CreateContentAsync_ShouldThrowExceptionIfUserDoesNotExist()
        {
            // Arrange
            var content = new Content { UserId = "invalid_user", Title = "Test Title" };
            // _mockUserService.Setup(s => s.CheckUserExistsAsync("invalid_user")).ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _contentService.CreateContentAsync(content));
            Assert.Equal("L'utilisateur avec l'ID 'invalid_user' n'existe pas.", exception.Message);
        }

        // [Fact]
        // public async Task GetContentByIdAsync_ShouldReturnContentWhenFound()
        // {
        //     // Arrange
        //     var content = new Content { id = "content123", content_id = "/content_id", Title = "Test Title" };

        //     // Simulation de la réponse ItemResponse<Content>
        //     var mockResponse = new Mock<ItemResponse<Content>>();
        //     mockResponse.SetupGet(r => r.Resource).Returns(content);

        //     // _mockContainer
        //     //     .Setup(c => c.ReadItemAsync<Content>("content123", new PartitionKey("/content_id"), null, default))
        //     //     .ReturnsAsync(mockResponse.Object);

        //     // Act
        //     var result = await _contentService.GetContentByIdAsync("content123");

        //     // Assert
        //     Assert.NotNull(result);
        //     Assert.Equal("Test Title", result?.Title);
        // }

        // [Fact]
        // public async Task GetContentByIdAsync_ShouldReturnNullWhenNotFound()
        // {
        //     // Arrange
        //     _mockContainer
        //         .Setup(c => c.ReadItemAsync<Content>("invalid_content", new PartitionKey("/content_id"), null, default))
        //         .ThrowsAsync(new CosmosException("Not found", HttpStatusCode.NotFound, 0, string.Empty, 0));

        //     // Act
        //     var result = await _contentService.GetContentByIdAsync("invalid_content");

        //     // Assert
        //     Assert.Null(result);
        // }

        // [Fact]
        // public async Task UpdateContentAsync_ShouldUpdateContentSuccessfully()
        // {
        //     // Arrange
        //     var existingContent = new Content { id = "content123", Title = "Old Title" };
        //     var updatedContent = new Content { id = "content123", Title = "New Title" };

        //     // Mock pour simuler la création de l'élément initial
        //     var createResponse = new Mock<ItemResponse<Content>>();
        //     createResponse.SetupGet(r => r.Resource).Returns(existingContent);

        //     _mockContainer
        //         .Setup(c => c.CreateItemAsync(It.IsAny<Content>(), It.IsAny<PartitionKey>(), null, default))
        //         .ReturnsAsync(createResponse.Object);

        //     // Mock pour simuler la lecture de l'élément existant
        //     var readResponse = new Mock<ItemResponse<Content>>();
        //     readResponse.SetupGet(r => r.Resource).Returns(existingContent);

        //     _mockContainer
        //         .Setup(c => c.ReadItemAsync<Content>("content123", new PartitionKey("/content_id"), null, default))
        //         .ReturnsAsync(readResponse.Object);

        //     // Mock pour simuler la mise à jour de l'élément
        //     var replaceResponse = new Mock<ItemResponse<Content>>();
        //     replaceResponse.SetupGet(r => r.Resource).Returns(updatedContent);

        //     _mockContainer
        //         .Setup(c => c.ReplaceItemAsync(It.IsAny<Content>(), "content123", new PartitionKey("/content_id"), null, default))
        //         .ReturnsAsync(replaceResponse.Object);

        //     // Act
        //     // Créez d'abord le contenu initial
        //     var createdContent = await _contentService.CreateContentAsync(existingContent);

        //     // Mettez ensuite le contenu à jour
        //     var result = await _contentService.UpdateContentAsync("content123", updatedContent);

        //     // Assert
        //     Assert.NotNull(createdContent);
        //     Assert.Equal("Old Title", createdContent.Title);

        //     Assert.NotNull(result);
        //     Assert.Equal("New Title", result.Title);
        // }

        // [Fact]
        // public async Task DeleteContentAsync_ShouldDeleteContentSuccessfully()
        // {
        //     // Arrange
        //     var mockItemResponse = new Mock<ItemResponse<Content>>();

        //     // Configurer le mock pour qu'il retourne un objet ItemResponse fictif
        //     _mockContainer
        //         .Setup(c => c.DeleteItemAsync<Content>("content123", new PartitionKey("/content_id"), null, default))
        //         .ReturnsAsync(mockItemResponse.Object); // Retourne un Task<ItemResponse<Content>>

        //     // Act & Assert
        //     await _contentService.DeleteContentAsync("content123");

        //     // Vérifie que DeleteItemAsync a été appelé une fois avec les bons paramètres
        //     _mockContainer.Verify(c => c.DeleteItemAsync<Content>("content123", new PartitionKey("/content_id"), null, default), Times.Once);
        // }

        [Fact]
        public async Task DeleteContentAsync_ShouldThrowExceptionIfNotFound()
        {
            // Arrange
            _mockContainer
                .Setup(c => c.DeleteItemAsync<Content>("invalid_content", It.IsAny<PartitionKey>(), null, default))
                .ThrowsAsync(new CosmosException("Not found", HttpStatusCode.NotFound, 0, string.Empty, 0));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _contentService.DeleteContentAsync("invalid_content"));
            Assert.Equal("Le contenu avec l'ID 'invalid_content' n'existe pas.", exception.Message);
        }
    }
}
