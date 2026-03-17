using GameInventoryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameInventoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(ITransactionService service) : ControllerBase
{
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetByUser(int userId, [FromQuery] int take = 50)
    {
        var (ok, message, data) = await service.GetByUserAsync(userId, take);
        if (!ok) return NotFound(message);
        return Ok(data);
    }
}
