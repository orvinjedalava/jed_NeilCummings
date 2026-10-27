namespace AuctionService.DTOs;

public class CreateAuctionDto
{
    public required string Make { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public required string Color { get; set; }
    public int Mileage { get; set; }
    public required string ImageUrl { get; set; }
    public required string Description { get; set; }
    public int? ReservePrice { get; set; }
    public DateTime AuctionEnd { get; set; }
}