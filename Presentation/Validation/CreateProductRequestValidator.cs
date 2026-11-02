using Application.DTOs;
using FluentValidation;

namespace Presentation.Validation;

public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequestDTO>
{
    public CreateProductRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().MaximumLength(200);
        RuleFor(request => request.Price).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Weight).GreaterThan(0);
        RuleFor(request => request.WarehouseNumber).GreaterThan(0);
        RuleFor(request => request.Type).IsInEnum();
    }
}