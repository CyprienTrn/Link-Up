using link_up.Models;
using link_up.Services;

namespace link_up.Routes
{
    public static class MediasRoutes
    {
        public static void MapMediasRoutes(this IEndpointRouteBuilder app)
        {
            // Route POST pour créer un utilisateur
            app.MapPost("/medias", async (Media media, MediaCosmosService mediaCosmosService) =>
            {
                var createdMedia = await mediaCosmosService.CreateMediaAsync(media);
                return Results.Created($"/medias/{createdMedia.id}", createdMedia);
            })
            .WithName("CreateMedia")
            .WithOpenApi();

            // Route GET pour récupérer tous les médias
            app.MapGet("/medias", async (MediaCosmosService mediaCosmosService) =>
            {
                var medias = await mediaCosmosService.GetAllMediasAsync();
                return medias;
            })
            .WithName("GetAllMedias")
            .WithOpenApi();
        }
    }
}