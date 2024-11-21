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
            // var accountEndpoint = configuration["CosmosDb:AccountEndpoint"];
            var accountEndpoint = "https://link-up-cosmosdb.documents.azure.com:443/";
            var accountKey = "lNKjo6NPpOttXtOCX1YStBM2b5hrMK14MzMGtFDGVngXCIA4jLizUO2CjWw0nsi0gX01xwBjgIWUACDbEPYx5g==";
            _cosmosClient = new CosmosClient(accountEndpoint, accountKey);

            _database = _cosmosClient.GetDatabase("link-up_bdd");
            _container = _database.GetContainer("user");
        }

        public async Task<List<LinkUpUser>> GetAllUtilisateursAsync()
        {
            try
            {
                var query = "SELECT * FROM user";

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

        public async Task<LinkUpUser> CreateUserAsync(LinkUpUser user)
        {
            try
            {
                var response = await _container.CreateItemAsync(user, new PartitionKey(user.Id));
                return response.Resource;
            }
            catch (CosmosException ex)
            {
                Console.WriteLine($"Error creating user: {ex.StatusCode} - {ex.Message}");
                throw;
            }
        }
    }
}