using AutoMapper;
using InventoryApi.Dtos;
using InventoryApi.Models;

namespace InventoryApi.MappingProfiles;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
    }
}