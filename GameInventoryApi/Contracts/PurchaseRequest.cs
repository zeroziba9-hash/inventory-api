using System.ComponentModel.DataAnnotations;

namespace GameInventoryApi.Contracts;

public class PurchaseRequest
{
    [Range(1, int.MaxValue)]
    public int UserId { get; set; }

    [Range(1, int.MaxValue)]
    public int ItemId { get; set; }

    [Range(1, 100)]
    public int Quantity { get; set; } = 1;
}
