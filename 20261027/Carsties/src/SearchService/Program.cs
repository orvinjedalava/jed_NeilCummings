using Meilisearch;
using SearchService.Data;
using SearchService.Endpoints;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    return new MeilisearchClient(
        config["Meilisearch:Url"],
        config["Meilisearch:ApiKey"]
    );
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/api/search", SearchEndpoints.GetSearchResults);

try
{
    await DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine($"Failed to seed search: {e.Message}");
}

app.Run();