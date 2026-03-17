using GameInventoryApi.Contracts;
using GameInventoryApi.Data;
using GameInventoryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameInventoryApi.Services;

public class InventoryService(GameDbContext db) : IInventoryService
{
    public async Task<object?> GetInventoryAsync(int userId)
    {
        var user = await db.Users.AsNoTracking()
            .Include(x => x.InventoryItems).ThenInclude(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null) return null;

        return new
        {
            user.Id,
            user.Nickname,
            user.Gold,
            items = user.InventoryItems.Select(x => new { x.ItemId, x.Item.Name, x.Quantity })
        };
    }

    public async Task<(bool ok, string message, object? data)> PurchaseAsync(PurchaseRequest request)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        var user = await db.Users.FirstOrDefaultAsync(x => x.Id == request.UserId);
        var item = await db.Items.FirstOrDefaultAsync(x => x.Id == request.ItemId);
        if (user is null || item is null) return (false, "User or Item not found", null);

        var totalPrice = checked(item.Price * request.Quantity);
        if (user.Gold < totalPrice) return (false, "Not enough gold", null);

        user.Gold -= totalPrice;
        var inv = await db.InventoryItems.FirstOrDefaultAsync(x => x.UserId == request.UserId && x.ItemId == request.ItemId);
        if (inv is null)
            db.InventoryItems.Add(new InventoryItem { UserId = request.UserId, ItemId = request.ItemId, Quantity = request.Quantity });
        else
            inv.Quantity = checked(inv.Quantity + request.Quantity);

        db.TransactionLogs.Add(new TransactionLog { UserId = request.UserId, Type = "BUY", ItemId = request.ItemId, Quantity = request.Quantity, GoldDelta = -totalPrice, CreatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();
        await tx.CommitAsync();

        return (true, "Purchase completed", new { user.Gold });
    }

    public async Task<(bool ok, string message, object? data)> UseAsync(UseItemRequest request)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        var inv = await db.InventoryItems.Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.ItemId == request.ItemId);
        if (inv is null) return (false, "Inventory item not found", null);
        if (inv.Quantity < request.Quantity) return (false, "Not enough item quantity", null);

        inv.Quantity -= request.Quantity;
        if (inv.Quantity == 0) db.InventoryItems.Remove(inv);

        db.TransactionLogs.Add(new TransactionLog { UserId = request.UserId, Type = "USE", ItemId = request.ItemId, Quantity = request.Quantity, GoldDelta = 0, CreatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();
        await tx.CommitAsync();

        return (true, "Item used", new { item = inv.Item.Name, remainingQuantity = inv.Quantity });
    }

    public async Task<(bool ok, string message, object? data)> SellAsync(SellItemRequest request)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        var inv = await db.InventoryItems.Include(x => x.User).Include(x => x.Item)
            .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.ItemId == request.ItemId);
        if (inv is null) return (false, "Inventory item not found", null);
        if (inv.Quantity < request.Quantity) return (false, "Not enough item quantity", null);

        var sellPrice = checked((inv.Item.Price / 2) * request.Quantity);
        inv.Quantity -= request.Quantity;
        inv.User.Gold = checked(inv.User.Gold + sellPrice);
        if (inv.Quantity == 0) db.InventoryItems.Remove(inv);

        db.TransactionLogs.Add(new TransactionLog { UserId = request.UserId, Type = "SELL", ItemId = request.ItemId, Quantity = request.Quantity, GoldDelta = sellPrice, CreatedAt = DateTime.UtcNow });
        await db.SaveChangesAsync();
        await tx.CommitAsync();

        return (true, "Item sold", new { earnedGold = sellPrice, inv.User.Gold });
    }
}
