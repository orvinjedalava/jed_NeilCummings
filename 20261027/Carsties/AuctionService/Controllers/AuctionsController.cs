using Microsoft.AspNetCore.Mvc;
using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController(AuctionDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAuctions()
    {
        var auctions = await context.Auctions
            .OrderBy(x => x.Item.Make)
            .ThenBy(x => x.Item.Model)
            .ProjectToType<AuctionDto>()
            .ToListAsync();
        
        return auctions.Adapt<List<AuctionDto>>();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuction(string id)
    {
        var auction = await context.Auctions
            .ProjectToType<AuctionDto>()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
            return NotFound();

        return auction;
    }

    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction(CreateAuctionDto createAuctionDto)
    {
        var auction = createAuctionDto.Adapt<Auction>();
        
        // TODO: add current user as seller
        auction.Seller = "TODO: seller";

        context.Auctions.Add(auction);

        await context.SaveChangesAsync();
        
        // HttpResponse will contain a Location header that tells how the client can get the newly created Auction
        return CreatedAtAction(nameof(GetAuction), new { id = auction.Id }, auction.Adapt<AuctionDto>());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAuction(string id, UpdateAuctionDto updateAuctionDto)
    {
        var auction = await context.Auctions
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (auction == null)
        {
            return NotFound();
        }

        if (auction.CurrentHighBid > 0)
        {
            return BadRequest("Cannot update an auction that has bids");
        }
        
        // TODO: Check seller is the same as the current user

        updateAuctionDto.Adapt(auction.Item);

        await context.SaveChangesAsync();
        
        return NoContent();
    }
}