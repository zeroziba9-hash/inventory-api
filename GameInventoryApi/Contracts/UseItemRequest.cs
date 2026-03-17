namespace GameInventoryApi.Contracts;

public record UseItemRequest(int UserId, int ItemId, int Quantity = 1);
