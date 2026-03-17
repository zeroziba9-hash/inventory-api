using GameInventoryApi.Contracts;
using GameInventoryApi.Data;
using GameInventoryApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameInventoryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController(GameDbContext db) : ControllerBase
{
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetInventory(int userId)
    {
        var user = await db.Users
            .AsNoTracking()
            .Include(x => x.InventoryItems)
                .ThenInclude(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null) return NotFound("User not found");

        var result = new
        {
            user.Id,
            user.Nickname,
            user.Gold,
            items = user.InventoryItems.Select(x => new
            {
                x.ItemId,
                x.Item.Name,
                x.Quantity
            })
        };

        return Ok(result);
    }

    [HttpPost("purchase")]
    public async Task<IActionResult> Purchase([FromBody] PurchaseRequest request)
    {
        await using var tx = await db.Database.BeginTransactionAsync();

        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
        var item = await db.Items.FirstOrDefaultAsync(x => x.Id == request.ItemId);

        if (user is null || item is null) return NotFound("User or Item not found");

        var totalPrice = checked(item.Price * request.Quantity);
        if (user.Gold < totalPrice) return BadRequest("Not enough gold");

        user.Gold -= totalPrice;

        var inv = await db.InventoryItems
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.ItemId == request.ItemId);

        if (inv is null)
        {
            db.InventoryItems.Add(new InventoryItem
            {
                UserId = request.UserId,
                ItemId = request.ItemId,
                Quantity = request.Quantity
            });
        }
        else
        {
            inv.Quantity = checked(inv.Quantity + request.Quantity);
        }

        db.TransactionLogs.Add(new TransactionLog
        {
            UserId = request.UserId,
            Type = "BUY",
            ItemId = request.ItemId,
            Quantity = request.Quantity,
            GoldDelta = -totalPrice,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
        await tx.CommitAsync();

        return Ok(new { message = "Purchase completed", user.Gold });
    }

    [HttpPost("use")]
    public async Task<IActionResult> UseItem([FromBody] UseItemRequest request)
    {
        await using var tx = await db.Database.BeginTransactionAsync();

        var inv = await db.InventoryItems
            .Include(x => x.User)
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.ItemId == request.ItemId);

        if (inv is null) return NotFound("Inventory item not found");
        if (inv.Quantity < request.Quantity) return BadRequest("Not enough item quantity");

        inv.Quantity -= request.Quantity;

        if (inv.Quantity == 0)
        {
            db.InventoryItems.Remove(inv);
        }

        db.TransactionLogs.Add(new TransactionLog
        {
            UserId = request.UserId,
            Type = "USE",
            ItemId = request.ItemId,
            Quantity = request.Quantity,
            GoldDelta = 0,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
        await tx.CommitAsync();

        return Ok(new
        {
            message = "Item used",
            item = inv.Item.Name,
            remainingQuantity = inv.Quantity
        });
    }

    [HttpPost("sell")]
    public async Task<IActionResult> SellItem([FromBody] SellItemRequest request)
    {
        await using var tx = await db.Database.BeginTransactionAsync();

        var inv = await db.InventoryItems
            .Include(x => x.User)
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.ItemId == request.ItemId);

        if (inv is null) return NotFound("Inventory item not found");
        if (inv.Quantity < request.Quantity) return BadRequest("Not enough item quantity");

        var sellPrice = checked((inv.Item.Price / 2) * request.Quantity);

        inv.Quantity -= request.Quantity;
        inv.User.Gold = checked(inv.User.Gold + sellPrice);

        if (inv.Quantity == 0)
        {
            db.InventoryItems.Remove(inv);
        }

        db.TransactionLogs.Add(new TransactionLog
        {
            UserId = request.UserId,
            Type = "SELL",
            ItemId = request.ItemId,
            Quantity = request.Quantity,
            GoldDelta = sellPrice,
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
        await tx.CommitAsync();

        return Ok(new { message = "Item sold", earnedGold = sellPrice, inv.User.Gold });
    }
}
