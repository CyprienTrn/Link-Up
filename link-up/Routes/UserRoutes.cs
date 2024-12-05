using link_up.Services;
using LinkUpUser = link_up.Models.User;

namespace link_up.Routes
{
    public static class UserRoutes
    {
        public static void MapUserRoutes(this IEndpointRouteBuilder app)
        {
            // Créer un nouvel utilisateur (POST /api/users)
            app.MapPost("/api/users", async (LinkUpUser user, UserCosmosService userCosmosService) =>
            {
                try
                {
                    var createdUser = await userCosmosService.CreateUserAsync(user);
                    return Results.Created($"/api/users/{createdUser.id}", createdUser);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            })
            .WithName("CreateUser")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Créer un nouvel utilisateur.";
                operation.Description = "Cet endpoint permet de créer un nouvel utilisateur dans le système.";
                return operation;
            })
            .Accepts<LinkUpUser>("application/json")
            .Produces<LinkUpUser>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

            // Récupérer tous les utilisateurs (GET /api/users)
            app.MapGet("/api/users", async (UserCosmosService userCosmosService) =>
            {
                try
                {
                    var users = await userCosmosService.GetAllUtilisateursAsync();
                    return Results.Ok(users);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            })
            .WithName("GetAllUsers")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Récupérer tous les utilisateurs.";
                operation.Description = "Cet endpoint permet de récupérer tous les utilisateurs présents dans le système.";
                return operation;
            })
            .Produces<IEnumerable<LinkUpUser>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

            // Récupérer un utilisateur par ID (GET /api/users/{id})
            app.MapGet("/api/users/{id}", async (string id, UserCosmosService userCosmosService) =>
            {
                try
                {
                    var user = await userCosmosService.GetUserByIdAsync(id);

                    if (user == null)
                    {
                        return Results.NotFound(new { message = $"Utilisateur avec l'ID {id} introuvable." });
                    }

                    return Results.Ok(user);
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            })
            .WithName("GetUserById")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Récupérer un utilisateur par ID.";
                operation.Description = "Cet endpoint permet de récupérer un utilisateur spécifique grâce à son ID.";
                return operation;
            })
            .Produces<LinkUpUser>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

            // Supprimer un utilisateur par ID (DELETE /api/users/{id})
            app.MapDelete("/api/users/{id}", async (string id, UserCosmosService userCosmosService) =>
            {
                try
                {
                    await userCosmosService.DeleteUserAsync(id);
                    return Results.NoContent();
                }
                catch (Exception ex)
                {
                    return Results.Problem(ex.Message);
                }
            })
            .WithName("DeleteUser")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Supprimer un utilisateur par ID.";
                operation.Description = "Cet endpoint permet de supprimer un utilisateur spécifique grâce à son ID.";
                return operation;
            })
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

            // Mettre à jour un utilisateur (PUT /api/users/{id})
            app.MapPut("/api/users/{id}", async (string id, LinkUpUser updatedUser, UserCosmosService userCosmosService) =>
            {
                try
                {
                    var updatedUserData = await userCosmosService.UpdateUserAsync(id, updatedUser);
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
            .WithName("UpdateUser")
            .WithOpenApi(operation =>
            {
                operation.Summary = "Mettre à jour un utilisateur par ID.";
                operation.Description = "Cet endpoint permet de mettre à jour un utilisateur existant grâce à son ID.";
                return operation;
            })
            .Accepts<LinkUpUser>("application/json")
            .Produces<LinkUpUser>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        }
    }
}
