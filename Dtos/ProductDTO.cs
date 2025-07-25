using InventoryApi.Enums;
namespace InventoryApi.Dtos;

public class ProductDTO
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public CategoryType Category { get; set; }
}