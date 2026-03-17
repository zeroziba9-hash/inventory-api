namespace GameInventoryApi.Services;

public interface ITransactionService
{
    Task<(bool ok, string message, object? data)> GetByUserAsync(int userId, int take);
}
