namespace InventoryApi.Dtos;

public class OrderItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal PriceAtPurchase { get; set; }
}