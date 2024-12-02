using link_up.Services;
using LinkUpUser = link_up.Models.User;

namespace link_up.Routes
{
    public static class UserRoutes
    {
        public static void MapUserRoutes(this IEndpointRouteBuilder app)
        {
            // Route POST pour créer un utilisateur
            app.MapPost("/utilisateurs", async (LinkUpUser user, UserCosmosService userCosmosService) =>
            {
                var createdUser = await userCosmosService.CreateUserAsync(user);
                return Results.Created($"/utilisateurs/{createdUser.id}", createdUser);
            })
            .WithName("CreateUtilisateur")
            .WithOpenApi();

            // Route GET pour récupérer tous les utilisateurs
            app.MapGet("/utilisateurs", async (UserCosmosService userCosmosService) =>
            {
                var utilisateurs = await userCosmosService.GetAllUtilisateursAsync();
                return utilisateurs;
            })
            .WithName("GetAllUtilisateurs")
            .WithOpenApi();

            // Route GET pour récupérer un utilisateur par ID
            app.MapGet("/utilisateurs/{id}", async (string id, string partitionKey, UserCosmosService userCosmosService) =>
            {
                try
                {
                    var user = await userCosmosService.GetUserByIdAsync(id, partitionKey);

                    if (user == null)
                    {
                        return Results.NotFound(new { message = $"User with ID {id} not found." });
                    }

                    return Results.Ok(user);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            })
            .WithName("GetUtilisateur")
            .WithOpenApi();

            // Route DELETE pour supprimer un utilisateur
            app.MapDelete("/utilisateurs/{id}", async (string id, string partitionKey, UserCosmosService userCosmosService) =>
            {
                try
                {
                    await userCosmosService.DeleteUserAsync(id, partitionKey);
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            })
            .WithName("DeleteUtilisateur")
            .WithOpenApi();

            // Route PUT pour mettre à jour un utilisateur
            app.MapPut("/utilisateurs/{id}", async (string id, LinkUpUser updatedUser, string partitionKey, UserCosmosService userCosmosService) =>
            {
                try
                {
                    var updatedUserData = await userCosmosService.UpdateUserAsync(id, updatedUser, partitionKey);
                    return Results.Ok(updatedUserData);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("not found"))
                    {
                        return Results.NotFound(new { message = ex.Message });
                    }
                    return Results.Problem(ex.Message);
                }
            })
            .WithName("UpdateUtilisateur")
            .WithOpenApi();
        }
    }
}
