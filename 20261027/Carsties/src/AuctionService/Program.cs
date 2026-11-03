using AuctionService.Data;
using AuctionService.Errors;
using Contracts;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Wolverine;
using Wolverine.RabbitMQ;

TypeAdapterConfig.GlobalSettings.Scan(typeof(Program).Assembly);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Host.UseWolverine(opts =>
{
    opts.UseRabbitMq(rabbit =>
        {
            rabbit.HostName = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
            rabbit.UserName = builder.Configuration["RabbitMQ:Username"] ?? "guest";
            rabbit.Password = builder.Configuration["RabbitMQ:Password"] ?? "guest";
        })
        .DeclareExchange("auction-created", ex => ex.ExchangeType = ExchangeType.Fanout)
        .AutoProvision(); // Let Wolverine auto-create missing and required infra

    opts.PublishMessage<AuctionCreated>().ToRabbitExchange("auction-created");
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