using Meilisearch;
using SearchService.Data;
using SearchService.Endpoints;
using SearchService.Services;
using Wolverine;
using Wolverine.RabbitMQ;

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
builder.Services.AddHttpClient<AuctionSvcHttpClient>()
    .AddStandardResilienceHandler(options =>
    {
        int maxTryCount = 5;
        options.TotalRequestTimeout.Timeout = TimeSpan.FromMinutes(3);
        options.Retry.MaxRetryAttempts = maxTryCount;
        options.Retry.Delay = TimeSpan.FromSeconds(10);
        options.Retry.OnRetry = args =>
        {
            Console.WriteLine($"Auction svc unavailable. Retry {args.AttemptNumber + 1}/5");
            return default;
        };
    });

builder.Host.UseWolverine(opts =>
{
    opts.UseRabbitMq(rabbit =>
        {
            rabbit.HostName = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
            rabbit.UserName = builder.Configuration["RabbitMQ:Username"] ?? "guest";
            rabbit.Password = builder.Configuration["RabbitMQ:Password"] ?? "guest";
        })
        .AutoProvision(); // Let Wolverine auto-create missing and required infra
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapGet("/api/search", SearchEndpoints.GetSearchResults);
app.MapGet("/api/search/{id}", SearchEndpoints.GetAuctionById);

try
{
    await DbInitializer.ConfigureIndex(app);
}
catch (Exception e)
{
    Console.WriteLine($"Failed to seed search: {e.Message}");
}

// Run the fetch from AuctionService as a background service.
// This will allow the SearchService to continue to Run and accept incoming HttpRequest
// while the background service retries until AuctionService becomes online
_ = Task.Run(async () =>
{
    try
    {
        await DbInitializer.FetchMissingAuctions(app);
    }
    catch (Exception e)
    {
        Console.WriteLine($"Failed to seed search: {e.Message}");
    }
});

app.Run();