using link_up.Services;
using LinkUpUser = link_up.Models.User;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSingleton<CosmosService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/utilisateurs", async (CosmosService cosmosService) =>
{
    var utilisateurs = await cosmosService.GetAllUtilisateursAsync();
    return utilisateurs;
})
.WithName("GetAllUtilisateurs")
.WithOpenApi();

app.MapPost("/utilisateurs", async (LinkUpUser user, CosmosService cosmosService) =>
{
    Console.WriteLine(user);
    var createdUser = await cosmosService.CreateUserAsync(user);
    return Results.Created($"/utilisateurs/{createdUser.Id}", createdUser);
})
.WithName("CreateUtilisateur")
.WithOpenApi();

app.Run();
