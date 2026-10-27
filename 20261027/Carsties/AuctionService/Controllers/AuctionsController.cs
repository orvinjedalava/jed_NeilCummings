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
}