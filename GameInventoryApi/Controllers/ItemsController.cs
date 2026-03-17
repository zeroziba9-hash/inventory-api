using GameInventoryApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameInventoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController(GameDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetItems()
    {
        var items = await db.Items
            .OrderBy(x => x.Id)
            .Select(x => new { x.Id, x.Name, x.Price })
            .ToListAsync();

        return Ok(items);
    }
}
