using FluentValidation;
using InventoryApi.Dtos;
using InventoryApi.Models;

namespace InventoryApi.Validators;

public class ProductValidator : AbstractValidator<ProductDto>
{
    public ProductValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Имя обязательно")
            .MaximumLength(100);
        RuleFor(p => p.Price)
            .GreaterThan(0).WithMessage("Цена должна быть больше 0");
        RuleFor(p => p.Stock)
            .GreaterThanOrEqualTo(0);
        
    }
}