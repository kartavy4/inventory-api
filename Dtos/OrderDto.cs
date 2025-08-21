using InventoryApi.Enums;

namespace InventoryApi.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItemDto> OrderItems { get; set; }
}