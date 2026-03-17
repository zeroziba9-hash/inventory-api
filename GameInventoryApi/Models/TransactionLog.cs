namespace GameInventoryApi.Models;

public class TransactionLog
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Type { get; set; } = string.Empty; // BUY, USE, SELL
    public int? ItemId { get; set; }
    public int Quantity { get; set; }
    public int GoldDelta { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
