using link_up.Models;
using link_up.Services;

namespace link_up.Routes
{
    public static class ContentRoutes
    {
        public static void MapContentRoutes(this IEndpointRouteBuilder app)
        {
            // Créer un nouveau contenu (POST /api/contents)
            app.MapPost("/api/contents", async (Content content, ContentCosmosService contentCosmosService) =>
            {
                var createdContent = await contentCosmosService.CreateContentAsync(content);
                return Results.Created($"/api/contents/{createdContent.id}", createdContent);
            })
            .WithName("CreateContent")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Créer un nouveau contenu.";
                operation.Description = "Cet endpoint permet de créer un nouveau contenu dans le système.";
                return operation;
            })
            .Accepts<Content>("application/json") // Spécifie que le body doit être un objet Content au format JSON
            .Produces<Content>(StatusCodes.Status201Created) // Réponse 201 avec un objet Content
            .ProducesProblem(StatusCodes.Status400BadRequest); // Documente les erreurs 400

            // Récupérer tous les contenus (GET /api/contents)
            app.MapGet("/api/contents", async (ContentCosmosService contentCosmosService) =>
            {
                var contents = await contentCosmosService.GetAllContentsAsync();
                return Results.Ok(contents);
            })
            .WithName("GetAllContents")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Récupérer tous les contenus.";
                operation.Description = "Cet endpoint permet de récupérer tous les contenus disponibles dans le système.";
                return operation;
            })
            .Produces<IEnumerable<Content>>(StatusCodes.Status200OK);

            // Récupérer un contenu par ID (GET /api/contents/{id})
            app.MapGet("/api/contents/{id}", async (string id, ContentCosmosService contentCosmosService) =>
            {
                var content = await contentCosmosService.GetContentByIdAsync(id);

                if (content == null)
                {
                    return Results.NotFound(new { message = $"Contenu avec l'ID {id} introuvable." });
                }

                return Results.Ok(content);
            })
            .WithName("GetContentById")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Récupérer un contenu par ID.";
                operation.Description = "Cet endpoint permet de récupérer un contenu spécifique à partir de son ID.";
                return operation;
            })
            .Produces<Content>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

            // Supprimer un contenu par ID (DELETE /api/contents/{id})
            app.MapDelete("/api/contents/{id}", async (string id, ContentCosmosService contentCosmosService) =>
            {
                try
                {
                    await contentCosmosService.DeleteContentAsync(id);
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            })
            .WithName("DeleteContent")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Supprimer un contenu par ID.";
                operation.Description = "Cet endpoint permet de supprimer un contenu spécifique en utilisant son ID.";
                return operation;
            })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound);

            // Mettre à jour un contenu (PUT /api/contents/{id})
            app.MapPut("/api/contents/{id}", async (string id, Content updatedContent, ContentCosmosService contentCosmosService) =>
            {
                try
                {
                    var updatedContentData = await contentCosmosService.UpdateContentAsync(id, updatedContent);
                    return Results.Ok(updatedContentData);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("introuvable"))
                    {
                        return Results.NotFound(new { message = ex.Message });
                    }
                    return Results.Problem(ex.Message);
                }
            })
            .WithName("UpdateContent")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Mettre à jour un contenu par ID.";
                operation.Description = "Cet endpoint permet de mettre à jour un contenu existant en utilisant son ID.";
                return operation;
            })
            .Accepts<Content>("application/json") // Spécifie que le body doit être un objet Content au format JSON
            .Produces<Content>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
