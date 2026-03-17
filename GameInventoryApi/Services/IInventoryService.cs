using GameInventoryApi.Contracts;

namespace GameInventoryApi.Services;

public interface IInventoryService
{
    Task<object?> GetInventoryAsync(int userId);
    Task<(bool ok, string message, object? data)> PurchaseAsync(PurchaseRequest request);
    Task<(bool ok, string message, object? data)> UseAsync(UseItemRequest request);
    Task<(bool ok, string message, object? data)> SellAsync(SellItemRequest request);
}
