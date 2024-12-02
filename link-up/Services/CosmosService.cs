using Microsoft.Azure.Cosmos;
using System.Net;
using UserApp = link_up.Models.User;
using link_up.DTO;

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
            // var cosmosSettings = configuration.GetSection("CosmosDb");
            var cosmosSettings = configuration.GetSection("CosmosDbCyp");
            string endpointUri = cosmosSettings["EndpointUri"];
            string primaryKey = cosmosSettings["PrimaryKey"];
            string databaseId = cosmosSettings["DatabaseId"];
            string containerId = cosmosSettings["ContainerId"];

            _cosmosClient = new CosmosClient(endpointUri, primaryKey, new CosmosClientOptions { ApplicationName = "LinkUpApp" });
            _database = _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            ContainerProperties containerProperties = new ContainerProperties()
            {
                Id = containerId,
                PartitionKeyPath = "/user_id",
            };
            _container = _database.CreateContainerIfNotExistsAsync(containerProperties).Result;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUtilisateursAsync()
        {
            var query = "SELECT c.id, c.Email, c.Name, c.IsPrivate, c.CreatedAt FROM c";
            var queryDefinition = new QueryDefinition(query);
            var queryResultSetIterator = _container.GetItemQueryIterator<UserDTO>(queryDefinition);

            var users = new List<UserDTO>();
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
                // Générer un ID si non fourni
                if (string.IsNullOrWhiteSpace(user.id))
                {
                    user.id = Guid.NewGuid().ToString();
                }

                // Assurez-vous que la clé de partition est définie
                if (string.IsNullOrWhiteSpace(user.user_id))
                {
                    user.user_id = "/user_id";
                }
                user.CreatedAt = DateTime.UtcNow;
                Console.WriteLine(user);
                Console.WriteLine("passe");
                var response = await _container.CreateItemAsync(user, new PartitionKey(user.user_id));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                throw new Exception($"User with ID {user.id} already exists.", ex);
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

            var response = await _container.ReplaceItemAsync(user, user.id.ToString(), new PartitionKey(user.id));
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
