using GameInventoryApi.Data;
using Microsoft.EntityFrameworkCore;

namespace GameInventoryApi.Services;

public class TransactionService(GameDbContext db) : ITransactionService
{
    public async Task<(bool ok, string message, object? data)> GetByUserAsync(int userId, int take)
    {
        var exists = await db.Users.AsNoTracking().AnyAsync(x => x.Id == userId);
        if (!exists) return (false, "User not found", null);

        var size = Math.Clamp(take, 1, 100);
        var logs = await db.TransactionLogs.AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(size)
            .ToListAsync();

        return (true, "ok", logs);
    }
}
