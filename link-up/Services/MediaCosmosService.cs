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
                PartitionKeyPath = "/media_id",
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
                throw new Exception($"Media with ID {media.id} already exists.", ex);
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

        public async Task<Media?> GetMediaByIdAsync(string mediaId)
        {
            try
            {
                // Lit l'élément de la base Cosmos DB
                var response = await _container.ReadItemAsync<Media>(mediaId, new PartitionKey(this._mediaPartitionKey));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                // Retourne null si le média n'est pas trouvé
                return null;
            }
            catch (CosmosException ex)
            {
                // Log et relance toute autre exception Cosmos
                Console.WriteLine($"An error occurred while retrieving the media: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteMediaAsync(string mediaId)
        {
            try
            {
                await _container.DeleteItemAsync<Media>(mediaId.ToString(), new PartitionKey(this._mediaPartitionKey));
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                throw new Exception($"User with ID {mediaId} not found.", ex);
            }
        }

    }
}
