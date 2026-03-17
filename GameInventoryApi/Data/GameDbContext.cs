using GameInventoryApi.Models;
using Microsoft.EntityFrameworkCore;

namespace GameInventoryApi.Data;

public class GameDbContext(DbContextOptions<GameDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<TransactionLog> TransactionLogs => Set<TransactionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventoryItem>()
            .HasKey(x => new { x.UserId, x.ItemId });

        modelBuilder.Entity<InventoryItem>()
            .HasOne(x => x.User)
            .WithMany(x => x.InventoryItems)
            .HasForeignKey(x => x.UserId);

        modelBuilder.Entity<InventoryItem>()
            .HasOne(x => x.Item)
            .WithMany(x => x.InventoryItems)
            .HasForeignKey(x => x.ItemId);

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Nickname = "Zero", Gold = 1000 }
        );

        modelBuilder.Entity<Item>().HasData(
            new Item { Id = 1, Name = "Potion", Price = 100 },
            new Item { Id = 2, Name = "Sword", Price = 500 }
        );

        modelBuilder.Entity<InventoryItem>().HasData(
            new InventoryItem { UserId = 1, ItemId = 1, Quantity = 3 }
        );
    }
}
