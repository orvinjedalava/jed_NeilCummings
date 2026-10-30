using Meilisearch;
using SearchService.Models;

namespace SearchService.Endpoints;

public static class SearchEndpoints
{
    public static async Task<IResult> GetSearchResults(MeilisearchClient client, string? searchTerm)
    {
        var query = new SearchQuery();

        var result = await client.Index("items")
            .SearchAsync<Item>(searchTerm ?? string.Empty, query);

        return Results.Ok(new
        {
            results = result.Hits
        });
    }
}