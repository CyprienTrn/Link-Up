// using Moq;
// using Microsoft.Extensions.Configuration;
// using Microsoft.Azure.Cosmos;
// using link_up.Models;
// using link_up.Services;
// using System;
// using System.Collections.Generic;
// using System.Net;
// using System.Threading.Tasks;
// using Xunit;

// public class MediaCosmosServiceTests
// {
//     private readonly Mock<Container> _mockContainer;
//     private readonly MediaCosmosService _mediaCosmosService;

//     public MediaCosmosServiceTests()
//     {
//         // Mock de la configuration
//         var mockConfig = new Mock<IConfiguration>();
//         mockConfig.SetupGet(c => c["CosmosDbCyp:EndpointUri"]).Returns("https://link-up-test.documents.azure.com:443/");
//         mockConfig.SetupGet(c => c["CosmosDbCyp:PrimaryKey"]).Returns("6TvtA3Vo4EfqiFyrggZh6WwVHpq2zOrUNC9eBvgyJbm2PhdVyRqw990MgI423I5L8k7XbOkQCi5uACDb7cJQhw==");
//         mockConfig.SetupGet(c => c["CosmosDbCyp:DatabaseId"]).Returns("link-up_bdd");
//         mockConfig.SetupGet(c => c["CosmosDbCyp:ContainerMediaId"]).Returns("media");
//         mockConfig.SetupGet(c => c["CosmosDbCyp:MediaPartitionKey"]).Returns("/media_id");

//         // Mock du Container Cosmos
//         _mockContainer = new Mock<Container>();

//         // Initialisation du service
//         _mediaCosmosService = new MediaCosmosService(mockConfig.Object);
//         _mediaCosmosService._container = _mockContainer.Object;
//     }

//     [Fact]
//     public async Task CreateMediaAsync_ShouldCreateMediaSuccessfully()
//     {
//         // Arrange
//         var media = new Media { id = "media123"};
//         var contentId = "content123";

//         // Mock de la réponse de Cosmos DB
//         var mockResponse = new Mock<ItemResponse<Media>>();
//         mockResponse.SetupGet(r => r.Resource).Returns(media);
//         _mockContainer.Setup(c => c.CreateItemAsync(It.IsAny<Media>(), It.IsAny<PartitionKey>(), null, default))
//                       .ReturnsAsync(mockResponse.Object);

//         // Act
//         var result = await _mediaCosmosService.CreateMediaAsync(media, contentId);

//         // Assert
//         Assert.NotNull(result);
//     }

//     [Fact]
//     public async Task GetAllMediasAsync_ShouldReturnListOfMedias()
//     {
//         // Arrange
//         var mediaList = new List<Media>
//         {
//             new Media { id = "1",  ContentId = "content123" },
//             new Media { id = "2",  ContentId = "content124" }
//         };

//         var mockResponse = new Mock<FeedResponse<Media>>();
//         mockResponse.Setup(r => r.GetEnumerator()).Returns(mediaList.GetEnumerator());

//         _mockContainer.Setup(c => c.GetItemQueryIterator<Media>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>()))
//                       .Returns(new Mock<FeedIterator<Media>>().Object);

//         // _mockContainer.Setup(c => c.GetItemQueryIterator<Media>(It.IsAny<QueryDefinition>()))
//         //               .Returns(new Mock<FeedIterator<Media>>().Object);

//         // Act
//         var result = await _mediaCosmosService.GetAllMediasAsync();

//         // Assert
//         Assert.NotNull(result);
//         Assert.Equal(2, result.Count());
//     }

//     [Fact]
//     public async Task GetMediaByIdAsync_ShouldReturnMediaWhenFound()
//     {
//         // Arrange
//         var mediaId = "content123";
//         var media = new Media { id = mediaId, ContentId = "content123" };

//         var mockResponse = new Mock<ItemResponse<Media>>();
//         mockResponse.SetupGet(r => r.Resource).Returns(media);

//         _mockContainer.Setup(c => c.ReadItemAsync<Media>(It.IsAny<string>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
//                       .ReturnsAsync(mockResponse.Object);

//         // Act
//         var result = await _mediaCosmosService.GetMediaByIdAsync(mediaId);

//         // Assert
//         Assert.NotNull(result);
//         Assert.Equal(mediaId, result.id);
//     }

//     [Fact]
//     public async Task GetMediaByIdAsync_ShouldReturnNullWhenMediaNotFound()
//     {
//         // Arrange
//         var mediaId = "non-existent-id";

//         _mockContainer.Setup(c => c.ReadItemAsync<Media>(It.IsAny<string>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
//                       .ThrowsAsync(new CosmosException("Not Found", HttpStatusCode.NotFound, 0, "", 0));

//         // Act
//         var result = await _mediaCosmosService.GetMediaByIdAsync(mediaId);

//         // Assert
//         Assert.Null(result);
//     }

//     [Fact]
//     public async Task DeleteMediaAsync_ShouldDeleteMediaSuccessfully()
//     {
//         // Arrange
//         var mediaId = "content123";

//         // _mockContainer.Setup(c => c.DeleteItemAsync<Media>(It.IsAny<string>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
//         //               .Returns(Task.CompletedTask);

//         // Act
//         await _mediaCosmosService.DeleteMediaAsync(mediaId);

//         // Assert
//         _mockContainer.Verify(c => c.DeleteItemAsync<Media>(mediaId, It.IsAny<PartitionKey>(), null, default), Times.Once);
//     }

//     [Fact]
//     public async Task DeleteMediaAsync_ShouldThrowExceptionIfMediaNotFound()
//     {
//         // Arrange
//         var mediaId = "non-existent-id";

//         _mockContainer.Setup(c => c.DeleteItemAsync<Media>(It.IsAny<string>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
//                       .ThrowsAsync(new CosmosException("Not Found", HttpStatusCode.NotFound, 0, "", 0));

//         // Act & Assert
//         var exception = await Assert.ThrowsAsync<Exception>(() => _mediaCosmosService.DeleteMediaAsync(mediaId));
//         Assert.Equal($"User with ID {mediaId} not found.", exception.Message);
//     }
// }
