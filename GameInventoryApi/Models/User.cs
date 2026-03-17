namespace GameInventoryApi.Models;

public class User
{
    public int Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public int Gold { get; set; } = 1000;

    public List<InventoryItem> InventoryItems { get; set; } = new();
}
