// using System.Collections.Generic;
// using System.Net;
// using System.Net.Http.Json;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Moq;
// using link_up.Models;
// using link_up.Services;
// using link_up.DTO;
// using Xunit;
// using Microsoft.Extensions.DependencyInjection;


// namespace link_up_tests.Routes
// {
//     public class UserRoutesTests : IClassFixture<WebApplicationFactory<Program>>
//     {
//         private readonly WebApplicationFactory<Program> _factory;

//         public UserRoutesTests()
//         {
//             _factory = new WebApplicationFactory<Program>();
//         }

//         [Fact]
//         public async Task CreateUser_ShouldReturnCreatedUser_WhenUserIsValid()
//         {
//             // Arrange
//             var mockService = new Mock<UserCosmosService>();
//             var newUser = new User { id = "123", Name = "John Doe", Email = "john@example.com" };
//             mockService.Setup(s => s.CreateUserAsync(It.IsAny<User>())).ReturnsAsync(newUser);

//             var app = _factory.WithWebHostBuilder(builder =>
//             {
//                 builder.ConfigureServices(services =>
//                 {
//                     services.AddScoped(_ => mockService.Object);
//                 });
//             });

//             var client = app.CreateClient();

//             // Act
//             var response = await client.PostAsJsonAsync("/", newUser);
//             var responseData = await response.Content.ReadFromJsonAsync<User>();

//             // Assert
//             Assert.Equal(HttpStatusCode.Created, response.StatusCode);
//             Assert.Equal(newUser.id, responseData?.id);
//             Assert.Equal(newUser.Name, responseData?.Name);
//         }

//         [Fact]
//         public async Task GetAllUsers_ShouldReturnListOfUsers()
//         {
//             // Arrange
//             var mockService = new Mock<UserCosmosService>();
//             var users = new List<UserDTO>
//             {
//                 new UserDTO { id = "123", Name = "John Doe", Email = "john@example.com" },
//                 new UserDTO { id = "456", Name = "Jane Doe", Email = "jane@example.com" }
//             };
//             mockService.Setup(s => s.GetAllUtilisateursAsync()).ReturnsAsync(users);

//             var app = _factory.WithWebHostBuilder(builder =>
//             {
//                 builder.ConfigureServices(services =>
//                 {
//                     services.AddScoped(_ => mockService.Object);
//                 });
//             });

//             var client = app.CreateClient();

//             // Act
//             var response = await client.GetAsync("/");
//             var responseData = await response.Content.ReadFromJsonAsync<List<UserDTO>>();

//             // Assert
//             Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//             Assert.Equal(2, responseData?.Count);
//             Assert.Equal("John Doe", responseData?[0].Name);
//         }

//         [Fact]
//         public async Task GetUser_ShouldReturnUser_WhenUserExists()
//         {
//             // Arrange
//             var mockService = new Mock<UserCosmosService>();
//             var user = new User { id = "123", Name = "John Doe", Email = "john@example.com" };
//             mockService.Setup(s => s.GetUserByIdAsync("123")).ReturnsAsync(user);

//             var app = _factory.WithWebHostBuilder(builder =>
//             {
//                 builder.ConfigureServices(services =>
//                 {
//                     services.AddScoped(_ => mockService.Object);
//                 });
//             });

//             var client = app.CreateClient();

//             // Act
//             var response = await client.GetAsync("/123");
//             var responseData = await response.Content.ReadFromJsonAsync<User>();

//             // Assert
//             Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//             Assert.Equal("John Doe", responseData?.Name);
//         }

//         [Fact]
//         public async Task DeleteUser_ShouldReturnNoContent_WhenUserIsDeleted()
//         {
//             // Arrange
//             var mockService = new Mock<UserCosmosService>();
//             mockService.Setup(s => s.DeleteUserAsync("123")).Returns(Task.CompletedTask);

//             var app = _factory.WithWebHostBuilder(builder =>
//             {
//                 builder.ConfigureServices(services =>
//                 {
//                     services.AddScoped(_ => mockService.Object);
//                 });
//             });

//             var client = app.CreateClient();

//             // Act
//             var response = await client.DeleteAsync("/123");

//             // Assert
//             Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
//         }

//         [Fact]
//         public async Task UpdateUser_ShouldReturnUpdatedUser_WhenUserExists()
//         {
//             // Arrange
//             var mockService = new Mock<UserCosmosService>();
//             var updatedUser = new User { id = "123", Name = "Updated Name", Email = "updated@example.com" };
//             mockService.Setup(s => s.UpdateUserAsync("123", It.IsAny<User>())).ReturnsAsync(updatedUser);

//             var app = _factory.WithWebHostBuilder(builder =>
//             {
//                 builder.ConfigureServices(services =>
//                 {
//                     services.AddScoped(_ => mockService.Object);
//                 });
//             });

//             var client = app.CreateClient();

//             // Act
//             var response = await client.PutAsJsonAsync("/123", updatedUser);
//             var responseData = await response.Content.ReadFromJsonAsync<User>();

//             // Assert
//             Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//             Assert.Equal("Updated Name", responseData?.Name);
//         }
//     }
// }
