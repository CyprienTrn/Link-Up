using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinkUpUser = link_up.Models.User;
using Microsoft.Azure.Cosmos;

namespace link_up.Services
{
    public class CosmosService
    {
        private CosmosClient _cosmosClient;
        private Database _database;
        private Container _container;

        public CosmosService(IConfiguration configuration)
        {
            var accountEndpoint = configuration["CosmosDb:AccountEndpoint"];
            var accountKey = configuration["CosmosDb:AccountKey"];
            _cosmosClient = new CosmosClient(accountEndpoint, accountKey);

            _database = _cosmosClient.GetDatabase(configuration["CosmosDb:DatabaseId"]);
            _container = _database.GetContainer(configuration["CosmosDb:ContainerId"]);
        }

        public async Task<List<LinkUpUser>> GetAllUtilisateursAsync()
        {
            try
            {
                var query = "SELECT * FROM c";

                var iterator = _container.GetItemQueryIterator<LinkUpUser>(new QueryDefinition(query));

                var utilisateurs = new List<LinkUpUser>();

                while (iterator.HasMoreResults)
                {
                    var response = await iterator.ReadNextAsync();

                    utilisateurs.AddRange(response);
                }

                return utilisateurs;
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Error retrieving users: {ex.StatusCode} - {ex.Message}");
                throw;
            }
        }

    }
}