using Mapster;
using Meilisearch;
using SearchService.Data;
using SearchService.Endpoints;
using SearchService.Handlers;
using SearchService.Services;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.RabbitMQ;

TypeAdapterConfig.GlobalSettings.Scan(typeof(Program).Assembly);

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

// Plug Wolverine (messaging/mediator framework) into the .NET generic host
builder.Host.UseWolverine(opts =>
{
    // Tell Wolverine to use RabbitMQ as its message transport (speaks AMQP 0-9-1)
    opts.UseRabbitMq(rabbit =>
        {
            // Connection settings for the broker; fall back to local dev defaults
            // ("guest/guest" only works against localhost on a default RabbitMQ)
            rabbit.HostName = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
            rabbit.UserName = builder.Configuration["RabbitMQ:Username"] ?? "guest";
            rabbit.Password = builder.Configuration["RabbitMQ:Password"] ?? "guest";
        })
        // Declare an exchange named "auction-created".
        // Fanout = copy every message to ALL queues bound to it (routing key ignored).
        // Other services can bind their own queues to receive the same event.
        // NOTE: Exchange lives in RabbitMQ broker. .DeclareExchange is indempotent.
        // If Search Service starts without the exchange yet, this creates it. 
        // If exchange is already existing, nothing happens.
        .DeclareExchange(
            "auction-created",
            ex => ex.ExchangeType = ExchangeType.Fanout)
        .DeclareExchange(
            "auction-updated",
            ex => ex.ExchangeType = ExchangeType.Fanout)
        .DeclareExchange(
            "auction-deleted",
            ex => ex.ExchangeType = ExchangeType.Fanout)
        .DeclareExchange(
            "auction-finished",
            ex => ex.ExchangeType = ExchangeType.Fanout)
        .DeclareExchange(
            "bid-placed",
            ex => ex.ExchangeType = ExchangeType.Fanout)
        
        // Bind this service's own queue to that exchange, so messages published
        // to "auction-created" are delivered into "search-auction-created"
        .BindExchange(
            "auction-created").ToQueue("search-auction-created")
        .BindExchange(
            "auction-updated").ToQueue("search-auction-updated")
        .BindExchange(
            "auction-deleted").ToQueue("search-auction-deleted")
        .BindExchange(
            "auction-finished").ToQueue("search-auction-finished")
        .BindExchange(
            "bid-placed").ToQueue("search-bid-placed")
        // On startup, create any missing exchanges/queues/bindings in RabbitMQ
        // so you don't have to create them manually (handy in dev; consider carefully in prod)
        .AutoProvision();

    // Make this service a consumer: Wolverine pulls messages from this queue
    // and routes each one to the matching handler, based on the message type
    // ListenToRabbitQueue only says where to read. Wolverine finds handler classes or methods by convention, 
    // for example a Handle(AuctionCreated message) method, in the application assembly.
    opts.ListenToRabbitQueue("search-auction-created");
    opts.ListenToRabbitQueue("search-auction-updated");
    opts.ListenToRabbitQueue("search-auction-deleted");
    opts.ListenToRabbitQueue("search-auction-finished");
    opts.ListenToRabbitQueue("search-bid-placed");

    // Configure retries when reading from queues
    opts.OnException<TransientSearchException>()
        .RetryWithCooldown(
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromMilliseconds(500),
            TimeSpan.FromSeconds(1)).Then.MoveToErrorQueue();
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