using Microsoft.AspNetCore.Mvc;
using LinkUpMedia = link_up.Models.Media;
using link_up.Services;
using System.Threading.Tasks;

namespace link_up.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly CosmosService _cosmosService;

        public MediaController(CosmosService cosmosService)
        {
            _cosmosService = cosmosService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMedia([FromBody] LinkUpMedia Media)
        {

            try
            {
                var createdMedia = await _cosmosService.CreateMediaAsync(Media);
                return CreatedAtAction(nameof(GetMediaById), new { id = createdMedia.Id }, createdMedia);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMediaById(string id)
        {
            // Ajoutez ici la logique pour récupérer un utilisateur par son ID, si nécessaire.
            return Ok(); // Placeholder.
        }
    }
}
