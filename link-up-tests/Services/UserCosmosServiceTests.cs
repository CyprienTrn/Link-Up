using Moq;
using Xunit;
using link_up.Services;
using link_up.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Threading.Tasks;
using System.Linq;
using link_up.DTO;
using System;
using UserApp = link_up.Models.User;


namespace link_up_tests
{
    public class UserCosmosServiceTests
    {
        private readonly Mock<CosmosClient> _mockCosmosClient;
        private readonly Mock<Container> _mockContainer;
        private readonly UserCosmosService _service;

        public UserCosmosServiceTests()
        {
            // Création de la configuration fictive
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(config => config.GetSection("CosmosDbCyp")["EndpointUri"]).Returns("https://link-up-test.documents.azure.com:443/");
            mockConfiguration.Setup(config => config.GetSection("CosmosDbCyp")["PrimaryKey"]).Returns("6TvtA3Vo4EfqiFyrggZh6WwVHpq2zOrUNC9eBvgyJbm2PhdVyRqw990MgI423I5L8k7XbOkQCi5uACDb7cJQhw==");
            mockConfiguration.Setup(config => config.GetSection("CosmosDbCyp")["DatabaseId"]).Returns("link-up_bdd");
            mockConfiguration.Setup(config => config.GetSection("CosmosDbCyp")["ContainerUserId"]).Returns("user");
            mockConfiguration.Setup(config => config.GetSection("CosmosDbCyp")["UserPartitionKey"]).Returns("/user_id");

            // Mocking du CosmosClient et de la container
            _mockCosmosClient = new Mock<CosmosClient>();
            _mockContainer = new Mock<Container>();

            // Initialisation du service
            _service = new UserCosmosService(mockConfiguration.Object){
                _cosmosClient = _mockCosmosClient.Object,
                _container = _mockContainer.Object
            };
        }

        [Fact]
        public async Task GetAllUtilisateursAsync_ShouldReturnUsers_WhenUsersExist()
        {
            // Arrange
            var users = new List<UserDTO>
            {
                new UserDTO { id = "1", Email = "test1@example.com", Name = "User 1", IsPrivate = false, CreatedAt = DateTime.UtcNow },
                new UserDTO { id = "2", Email = "test2@example.com", Name = "User 2", IsPrivate = true, CreatedAt = DateTime.UtcNow }
            };

            var mockResponse = new Mock<FeedResponse<UserDTO>>();
            mockResponse.Setup(r => r.GetEnumerator()).Returns(users.GetEnumerator());
            var iteratorMock = new Mock<FeedIterator<UserDTO>>();
            iteratorMock.Setup(i => i.ReadNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(mockResponse.Object);

            _mockContainer.Setup(c => c.GetItemQueryIterator<UserDTO>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>())).Returns(iteratorMock.Object);

            // Act
            var result = await _service.GetAllUtilisateursAsync();

            // Assert
            Assert.NotNull(result);
            // Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task IsEmailValidAndUniqueAsync_ShouldReturnTrue_WhenEmailIsValidAndUnique()
        {
            // Arrange
            string email = "unique@example.com";

            var mockResponse = new Mock<FeedResponse<int>>();
            mockResponse.Setup(r => r.GetEnumerator()).Returns(new List<int> { 0 }.GetEnumerator()); // Aucun utilisateur existant
            var iteratorMock = new Mock<FeedIterator<int>>();
            iteratorMock.Setup(i => i.ReadNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(mockResponse.Object);

            _mockContainer.Setup(c => c.GetItemQueryIterator<int>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>())).Returns(iteratorMock.Object);

            // Act
            var result = await _service.IsEmailValidAndUniqueAsync(email);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldCreateUser_WhenEmailIsValid()
        {
            // Arrange
            var user = new UserApp
            {
                Email = "newuser@example.com",
                Name = "New User",
                IsPrivate = true
            };

            var mockResponse = new Mock<ItemResponse<UserApp>>();
            mockResponse.Setup(r => r.Resource).Returns(user);

            _mockContainer.Setup(c => c.CreateItemAsync(It.IsAny<UserApp>(), It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse.Object);

            // Act
            var result = await _service.CreateUserAsync(user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = "1";
            var user = new UserApp
            {
                id = userId,
                Email = "existinguser@example.com",
                Name = "Existing User",
                IsPrivate = true,
                CreatedAt = DateTime.UtcNow
            };

            var mockResponse = new Mock<ItemResponse<UserApp>>();
            mockResponse.Setup(r => r.Resource).Returns(user);

            _mockContainer.Setup(c => c.ReadItemAsync<UserApp>(userId, It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse.Object);

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.id);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldThrowNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = "nonexistent";
            _mockContainer.Setup(c => c.DeleteItemAsync<UserApp>(userId, It.IsAny<PartitionKey>(), It.IsAny<ItemRequestOptions>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new CosmosException("Not Found", HttpStatusCode.NotFound, 0, string.Empty, 0));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.DeleteUserAsync(userId));
        }
    }
}
