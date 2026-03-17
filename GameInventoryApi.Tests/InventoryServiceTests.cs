using GameInventoryApi.Contracts;
using GameInventoryApi.Data;
using GameInventoryApi.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GameInventoryApi.Tests;

public class InventoryServiceTests
{
    private static (GameDbContext db, SqliteConnection conn) CreateDb()
    {
        var conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();

        var options = new DbContextOptionsBuilder<GameDbContext>()
            .UseSqlite(conn)
            .Options;

        var db = new GameDbContext(options);
        db.Database.EnsureCreated();
        return (db, conn);
    }

    [Fact]
    public async Task Purchase_ShouldDecreaseGold_AndIncreaseInventory()
    {
        var (db, conn) = CreateDb();
        await using var _db = db;
        await using var _conn = conn;

        var service = new InventoryService(db);
        var (ok, _, _) = await service.PurchaseAsync(new PurchaseRequest { UserId = 1, ItemId = 1, Quantity = 2 });

        Assert.True(ok);
        Assert.Equal(800, db.Users.Single(x => x.Id == 1).Gold);
        Assert.Equal(5, db.InventoryItems.Single(x => x.UserId == 1 && x.ItemId == 1).Quantity);
    }

    [Fact]
    public async Task Sell_ShouldIncreaseGold_AndDecreaseInventory()
    {
        var (db, conn) = CreateDb();
        await using var _db = db;
        await using var _conn = conn;

        var service = new InventoryService(db);
        var (ok, _, _) = await service.SellAsync(new SellItemRequest { UserId = 1, ItemId = 1, Quantity = 1 });

        Assert.True(ok);
        Assert.Equal(1050, db.Users.Single(x => x.Id == 1).Gold);
        Assert.Equal(2, db.InventoryItems.Single(x => x.UserId == 1 && x.ItemId == 1).Quantity);
    }
}
