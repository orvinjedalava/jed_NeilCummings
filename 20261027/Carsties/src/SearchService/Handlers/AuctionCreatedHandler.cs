using Contracts;
using Mapster;
using Meilisearch;
using SearchService.Models;

namespace SearchService.Handlers;

/// <summary>
/// IMPORTANT: WolverineFX works on conventions for runtime code generation.
/// Please make sure class ends with "Handler" and function name is "Handle"
/// </summary>
public class AuctionCreatedHandler
{
    public async Task Handle(AuctionCreated message, MeilisearchClient client)
    {
        var item = message.Adapt<Item>();
        await client.Index("items").AddDocumentsAsync([item]);
    }
}