using Microsoft.AspNetCore.Mvc;
using LinkUpFollower = link_up.Models.Follower;
using link_up.Services;
using System.Threading.Tasks;

namespace link_up.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FollowerController : ControllerBase
    {
        private readonly CosmosService _cosmosService;

        public FollowerController(CosmosService cosmosService)
        {
            _cosmosService = cosmosService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateFollower([FromBody] LinkUpFollower Follower)
        {

            try
            {
                var createdFollower = await _cosmosService.CreateFollowerAsync(Follower);
                return CreatedAtAction(nameof(GetFollowerById), new { id = createdFollower.Id }, createdFollower);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFollowerById(string id)
        {
            // Ajoutez ici la logique pour récupérer un utilisateur par son ID, si nécessaire.
            return Ok(); // Placeholder.
        }
    }
}
