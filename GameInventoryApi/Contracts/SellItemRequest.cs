namespace GameInventoryApi.Contracts;

public record SellItemRequest(int UserId, int ItemId, int Quantity = 1);
