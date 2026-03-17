using GameInventoryApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameInventoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransactionsController(GameDbContext db) : ControllerBase
{
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetByUser(int userId, [FromQuery] int take = 50)
    {
        var exists = await db.Users.AsNoTracking().AnyAsync(x => x.Id == userId);
        if (!exists) return NotFound("User not found");

        var size = Math.Clamp(take, 1, 100);

        var logs = await db.TransactionLogs
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(size)
            .ToListAsync();

        return Ok(logs);
    }
}
