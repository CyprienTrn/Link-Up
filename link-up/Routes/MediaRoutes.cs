using link_up.Services;
using link_up.Models;

namespace link_up.Routes
{
    public static class MediasRoutes
    {
        public static void MapMediaRoutes(this IEndpointRouteBuilder app)
        {
            // Récupérer tous les médias (GET /api/medias)
            app.MapGet("/api/medias", async (MediaCosmosService mediaCosmosService) =>
            {
                var medias = await mediaCosmosService.GetAllMediasAsync();
                return Results.Ok(medias);
            })
            .WithName("GetAllMedias")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Récupérer tous les médias.";
                operation.Description = "Cet endpoint permet de récupérer tous les médias disponibles dans le système.";
                return operation;
            })
            .Produces<IEnumerable<Media>>(StatusCodes.Status200OK);

            // Récupérer un média par ID (GET /api/medias/{id})
            app.MapGet("/api/medias/{id}", async (string id, MediaCosmosService mediaCosmosService) =>
            {
                try
                {
                    var media = await mediaCosmosService.GetMediaByIdAsync(id);

                    if (media == null)
                    {
                        return Results.NotFound(new { message = $"Média avec l'ID {id} introuvable." });
                    }

                    return Results.Ok(media);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            })
            .WithName("GetMediaById")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Récupérer un média par ID.";
                operation.Description = "Cet endpoint permet de récupérer un média spécifique à partir de son ID.";
                return operation;
            })
            .Produces<Media>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

            // Supprimer un média par ID (DELETE /api/medias/{id})
            app.MapDelete("/api/medias/{id}", async (string id, MediaCosmosService mediaCosmosService) =>
            {
                try
                {
                    await mediaCosmosService.DeleteMediaAsync(id);
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            })
            .WithName("DeleteMedia")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Supprimer un média par ID.";
                operation.Description = "Cet endpoint permet de supprimer un média spécifique en utilisant son ID.";
                return operation;
            })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
