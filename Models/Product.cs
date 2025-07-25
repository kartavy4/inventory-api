using InventoryApi.Enums;
namespace InventoryApi.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    
    public CategoryType Category { get; set; }
}