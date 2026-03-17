using GameInventoryApi.Contracts;
using GameInventoryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameInventoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController(IInventoryService service) : ControllerBase
{
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetInventory(int userId)
    {
        var result = await service.GetInventoryAsync(userId);
        return result is null ? NotFound("User not found") : Ok(result);
    }

    [Authorize]
    [HttpPost("purchase")]
    public async Task<IActionResult> Purchase([FromBody] PurchaseRequest request)
    {
        var (ok, message, data) = await service.PurchaseAsync(request);
        if (!ok) return BadRequest(message);
        return Ok(new { message, data });
    }

    [Authorize]
    [HttpPost("use")]
    public async Task<IActionResult> UseItem([FromBody] UseItemRequest request)
    {
        var (ok, message, data) = await service.UseAsync(request);
        if (!ok) return BadRequest(message);
        return Ok(new { message, data });
    }

    [Authorize]
    [HttpPost("sell")]
    public async Task<IActionResult> SellItem([FromBody] SellItemRequest request)
    {
        var (ok, message, data) = await service.SellAsync(request);
        if (!ok) return BadRequest(message);
        return Ok(new { message, data });
    }
}
