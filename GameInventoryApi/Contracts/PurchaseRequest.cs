namespace GameInventoryApi.Contracts;

public record PurchaseRequest(int UserId, int ItemId, int Quantity = 1);
