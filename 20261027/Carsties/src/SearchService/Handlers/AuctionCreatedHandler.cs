using Contracts;
using Mapster;
using Meilisearch;
using SearchService.Models;

namespace SearchService.Handlers;

public class TransientSearchException(string message) : Exception(message);

/// <summary>
/// IMPORTANT: WolverineFX works on conventions for runtime code generation.
/// Please make sure class ends with "Handler" and function name is "Handle"
/// </summary>
public class AuctionCreatedHandler
{
    // Temporary: Used to simulate transient errors - demo purposes
    private static readonly HashSet<string> FailedOnce = [];
    
    public async Task Handle(AuctionCreated message, MeilisearchClient client)
    {
        // Temporary: Used to simulate transient errors - demo purposes
        if (message.Make == "fail-once" && FailedOnce.Add(message.Id))
        {
            throw new TransientSearchException($"Simulated failure for {message.Id}");
        }
        
        var item = message.Adapt<Item>();
        await client.Index("items").AddDocumentsAsync([item]);
    }
}