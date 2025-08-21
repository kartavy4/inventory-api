
using InventoryApi.Models;

namespace InventoryApi.Dtos;

public class ProductDto
{
    public string Name { get; set; } = String.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string CategoryName { get; set; } = String.Empty;
}

public class ProductCreateDto
{
    public string Name { get; set; } = String.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}

public class ProductUpdateDto
{
    public string Name { get; set; } = String.Empty;
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
}
