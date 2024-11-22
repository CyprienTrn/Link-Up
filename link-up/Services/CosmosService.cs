using Microsoft.Azure.Cosmos;
using System.Net;
using UserApp = link_up.Models.User;

namespace link_up.Services
{
    public class CosmosService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Database _database;
        private readonly Container _container;

        public CosmosService(IConfiguration configuration)
        {
            // on charge la configuration depuis le appsettings.json
            var cosmosSettings = configuration.GetSection("CosmosDb");
            string endpointUri = cosmosSettings["EndpointUri"];
            string primaryKey = cosmosSettings["PrimaryKey"];
            string databaseId = cosmosSettings["DatabaseId"];
            string containerId = cosmosSettings["ContainerId"];

            _cosmosClient = new CosmosClient(endpointUri, primaryKey, new CosmosClientOptions { ApplicationName = "LinkUpApp" });
            _database = _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            _container = _database.CreateContainerIfNotExistsAsync(containerId, "/Id").Result;
        }

        public async Task<IEnumerable<User>> GetAllUtilisateursAsync()
        {
            var query = "SELECT * FROM c";
            var queryDefinition = new QueryDefinition(query);
            var queryResultSetIterator = _container.GetItemQueryIterator<User>(queryDefinition);

            var users = new List<User>();
            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                users.AddRange(currentResultSet);
            }
            return users;
        }

        public async Task<UserApp> CreateUserAsync(UserApp user)
        {
            try
            {
                user.CreatedAt = DateTime.UtcNow;
                var response = await _container.CreateItemAsync(user, new PartitionKey(user.Id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                throw new Exception($"User with ID {user.Id} already exists.", ex);
            }
        }

        public async Task<UserApp?> GetUserByIdAsync(int userId)
        {
            try
            {
                var response = await _container.ReadItemAsync<UserApp>(userId.ToString(), new PartitionKey(userId));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<UserApp> UpdateUserAsync(int userId, UserApp updatedUser)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null) throw new Exception($"User with ID {userId} not found.");

            user.Email = updatedUser.Email ?? user.Email;
            user.Name = updatedUser.Name ?? user.Name;
            user.IsPrivate = updatedUser.IsPrivate;

            var response = await _container.ReplaceItemAsync(user, user.Id.ToString(), new PartitionKey(user.Id));
            return response.Resource;
        }

        public async Task DeleteUserAsync(int userId)
        {
            try
            {
                await _container.DeleteItemAsync<UserApp>(userId.ToString(), new PartitionKey(userId));
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception($"User with ID {userId} not found.", ex);
            }
        }
    }
}
