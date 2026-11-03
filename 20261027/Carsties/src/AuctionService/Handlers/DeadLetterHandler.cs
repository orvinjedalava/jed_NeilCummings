using Contracts;

namespace AuctionService.Handlers;

/// <summary>
/// IMPORTANT: In real enterprise, this handler should be in charge of starting dead letter workflows
/// For demo purposes, we just log a message saying we received a message from deal letter queue
/// </summary>
/// <param name="logger"></param>
public class DeadLetterHandler(ILogger<DeadLetterHandler> logger)
{
    public Task Handle(AuctionCreated message)
    {
        logger.LogError("Dead letter handler triggered for {AuctionId} {Make} {Model}",
            message.Id, message.Make, message.Model);
        
        return Task.CompletedTask;
    }
}