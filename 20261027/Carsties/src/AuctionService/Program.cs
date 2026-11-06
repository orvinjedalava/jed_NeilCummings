using AuctionService.Data;
using AuctionService.Errors;
using Contracts;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Wolverine.RabbitMQ;

TypeAdapterConfig.GlobalSettings.Scan(typeof(Program).Assembly);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var connString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connString)) 
    throw new Exception("Connection string is empty");

builder.Services.AddDbContextWithWolverineIntegration<AuctionDbContext>(options =>
{
    options.UseNpgsql(connString);
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // tells our AuctionService where to go to validate the token ( JWT )
        options.Authority = builder.Configuration["IdentityServiceUrl"];
        options.RequireHttpsMetadata = false;
        // we are not returning audience with our token, so we're not able to validate audience
        options.TokenValidationParameters.ValidateAudience = false;
        // this is the custom claim we used that contains the name of the user
        options.TokenValidationParameters.NameClaimType = "username";
    });

builder.Host.UseWolverine(opts =>
{
    // Setup durable queues by saving messages to Postgres and creating transactions via EFCore
    opts.PersistMessagesWithPostgresql(connString, "auctions_rmq");
    opts.UseEntityFrameworkCoreTransactions();
    opts.Policies.UseDurableOutboxOnAllSendingEndpoints();
    
    opts.UseRabbitMq(rabbit =>
        {
            rabbit.HostName = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
            rabbit.UserName = builder.Configuration["RabbitMQ:Username"] ?? "guest";
            rabbit.Password = builder.Configuration["RabbitMQ:Password"] ?? "guest";
        })
        .DeclareExchange("auction-created", ex => ex.ExchangeType = ExchangeType.Fanout)
        .DeclareExchange("auction-updated", ex => ex.ExchangeType = ExchangeType.Fanout)
        .DeclareExchange("auction-deleted", ex => ex.ExchangeType = ExchangeType.Fanout)
        .DeclareExchange("auction-finished", ex => ex.ExchangeType = ExchangeType.Fanout)
        .DeclareExchange("bid-placed", ex => ex.ExchangeType = ExchangeType.Fanout)
        .BindExchange("auction-finished").ToQueue("auction-auction-finished")
        .BindExchange("bid-placed").ToQueue("auction-bid-placed")
        .AutoProvision(); // Let Wolverine auto-create missing and required infra

    opts.PublishMessage<AuctionCreated>().ToRabbitExchange("auction-created");
    opts.PublishMessage<AuctionUpdated>().ToRabbitExchange("auction-updated");
    opts.PublishMessage<AuctionDeleted>().ToRabbitExchange("auction-deleted");

    opts.ListenToRabbitQueue("wolverine-dead-letter-queue");
    opts.ListenToRabbitQueue("auction-auction-finished");
    opts.ListenToRabbitQueue("auction-bid-placed");
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// app.UseAuthorization();
app.UseExceptionHandler();
app.MapControllers();

try
{
    DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine($"Error initializing database: {e.Message}");
}

app.Run();