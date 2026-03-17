namespace GameInventoryApi.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }

    public List<InventoryItem> InventoryItems { get; set; } = new();
}
