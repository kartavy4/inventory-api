using AutoMapper;
using InventoryApi.Models;
using InventoryApi.Dtos;

namespace InventoryApi.MappingProfiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<ProductDTO, Product>();
        CreateMap<Product, ProductDTO>();
    }
}