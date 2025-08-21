using InventoryApi.Models;

namespace InventoryApi.Dtos;

public class OrderCreateDto
{
    public List<OrderItem> Items { get; set; }
}