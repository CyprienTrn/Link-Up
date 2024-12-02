using Microsoft.AspNetCore.Mvc;
using LinkUpUser = link_up.Models.User;
using link_up.Services;

namespace link_up.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly CosmosService _cosmosService;

        public UserController(CosmosService cosmosService)
        {
            _cosmosService = cosmosService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] LinkUpUser user)
        {

            try
            {
                var createdUser = await _cosmosService.CreateUserAsync(user);
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.id }, createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            // Ajoutez ici la logique pour récupérer un utilisateur par son ID, si nécessaire.
            return Ok(); // Placeholder.
        }
    }
}
