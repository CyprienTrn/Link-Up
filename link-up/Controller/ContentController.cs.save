using Microsoft.AspNetCore.Mvc;
using LinkUpContent = link_up.Models.Content;
using link_up.Services;
using System.Threading.Tasks;

namespace link_up.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContentController : ControllerBase
    {
        private readonly CosmosService _cosmosService;

        public ContentController(CosmosService cosmosService)
        {
            _cosmosService = cosmosService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateContent([FromBody] LinkUpContent Content)
        {

            try
            {
                var createdContent = await _cosmosService.CreateContentAsync(Content);
                return CreatedAtAction(nameof(GetContentById), new { id = createdContent.Id }, createdContent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContentById(string id)
        {
            // Ajoutez ici la logique pour récupérer un utilisateur par son ID, si nécessaire.
            return Ok(); // Placeholder.
        }
    }
}
