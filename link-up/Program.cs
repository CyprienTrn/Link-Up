using link_up.Services;
using link_up.Routes;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSingleton<UserCosmosService>();
builder.Services.AddSingleton<MediaCosmosService>();
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

// on ajoute toutes les routes de l'api de l'utilisateur
app.MapUserRoutes();

// on ajoute toutes les routes de l'api du média
app.MapMediasRoutes();

app.Run();
