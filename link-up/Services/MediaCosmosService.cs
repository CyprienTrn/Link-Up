using Microsoft.Azure.Cosmos;
using System.Net;
using link_up.Models;

namespace link_up.Services
{
    public class MediaCosmosService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Database _database;
        private readonly Container _container;
        private string _mediaPartitionKey;


        public MediaCosmosService(IConfiguration configuration)
        {
            // on charge la configuration depuis le appsettings.json
            // var cosmosSettings = configuration.GetSection("CosmosDb");
            var cosmosSettings = configuration.GetSection("CosmosDbCyp");
            string endpointUri = cosmosSettings["EndpointUri"];
            string primaryKey = cosmosSettings["PrimaryKey"];
            string databaseId = cosmosSettings["DatabaseId"];
            string containerId = cosmosSettings["ContainerMediaId"];
            _mediaPartitionKey = cosmosSettings["MediaPartitionKey"];

            _cosmosClient = new CosmosClient(endpointUri, primaryKey, new CosmosClientOptions { ApplicationName = "LinkUpApp" });
            _database = _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            ContainerProperties containerProperties = new ContainerProperties()
            {
                Id = containerId,
                PartitionKeyPath = "/media",
            };
            _container = _database.CreateContainerIfNotExistsAsync(containerProperties).Result;
        }

        public async Task<Media> CreateMediaAsync(Media media)
        {
            try
            {
                // Générer un ID si non fourni
                if (string.IsNullOrWhiteSpace(media.id))
                {
                    media.id = Guid.NewGuid().ToString();
                }

                // Assurez-vous que la clé de partition est définie
                if (string.IsNullOrWhiteSpace(media.media_id))
                {
                    media.media_id = this._mediaPartitionKey;
                }

                media.UploadedAt = DateTime.UtcNow;
                var response = await _container.CreateItemAsync(media, new PartitionKey(media.media_id));

                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                throw new Exception($"User with ID {media.id} already exists.", ex);
            }
        }

        public async Task<IEnumerable<Media>> GetAllMediasAsync()
        {
            var query = "SELECT * FROM c";
            var queryDefinition = new QueryDefinition(query);
            var queryResultSetIterator = _container.GetItemQueryIterator<Media>(queryDefinition);

            var users = new List<Media>();
            while (queryResultSetIterator.HasMoreResults)
            {
                var currentResultSet = await queryResultSetIterator.ReadNextAsync();
                users.AddRange(currentResultSet);
            }
            return users;
        }

    }
}
