namespace GameInventoryApi.Models;

public class InventoryItem
{
    public int UserId { get; set; }
    public User User { get; set; } = default!;

    public int ItemId { get; set; }
    public Item Item { get; set; } = default!;

    public int Quantity { get; set; }
}
