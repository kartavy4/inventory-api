using AutoMapper;
using InventoryApi.Dtos;
using InventoryApi.Models;

namespace InventoryApi.MappingProfiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>().ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.Items));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.PriceAtPurchase, opt => opt.MapFrom(src => src.Product.Price));
        CreateMap<OrderDto, Order>();
        CreateMap<OrderItemDto, OrderItem>();
    }
}