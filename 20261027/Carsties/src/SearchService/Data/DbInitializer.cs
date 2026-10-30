using System.Text.Json;
using Meilisearch;
using SearchService.Models;

namespace SearchService.Data;

public class DbInitializer
{
    private const string IndexUid = "items";

    public static async Task InitDb(WebApplication app)
    {
        var client = app.Services.GetRequiredService<MeilisearchClient>();

        if (await IndexHasDocuments(client))
        {
            Console.WriteLine("Search index already has documents. Seeding skipped.");
            return;
        }
        
        var path = Path.Combine(AppContext.BaseDirectory, "Data", "auctions.json");
        await using var stream = File.OpenRead(path);
        var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        var items = await JsonSerializer.DeserializeAsync<List<Item>>(stream, options);

        if (items?.Count == 0)
        {
            Console.WriteLine("No items found. Seeding skipped.");
            return;
        }
        
        var index = client.Index(IndexUid);

        var addTask = await index.AddDocumentsAsync(items, primaryKey: "id");
        await client.WaitForTaskAsync(addTask.TaskUid);

        var settingsTask = await index.UpdateSettingsAsync(new Settings()
        {
            SearchableAttributes = ["make", "model", "description"],
            FilterableAttributes = ["seller", "winner", "status", "auctionEnd"],
            SortableAttributes = ["auctionEnd", "currentHighBid", "createdAt", "updatedAt", "make", "model"]
        });

        await client.WaitForTaskAsync(settingsTask.TaskUid);
        
        Console.WriteLine($"Search index populated with {items?.Count} items");
    }

    private static async Task<bool> IndexHasDocuments(MeilisearchClient client)
    {
        var indexes = await client.GetAllIndexesAsync();
        if (indexes.Results.All(i => i.Uid != IndexUid))
            return false;

        var stats = await client.Index(IndexUid).GetStatsAsync();

        return stats.NumberOfDocuments > 0;
    }
}