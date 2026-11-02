using Application.DTOs;
using FluentValidation;

namespace Presentation.Validation;

public sealed class GetProductsRequestValidator : AbstractValidator<GetProductsRequestDTO>
{
    public GetProductsRequestValidator()
    {
        RuleFor(request => request.Page).GreaterThanOrEqualTo(1);
        RuleFor(request => request.PageSize).InclusiveBetween(1, 100);
        RuleFor(request => request.WarehouseNumber).GreaterThan(0).When(request => request.WarehouseNumber.HasValue);
        RuleFor(request => request.Type).IsInEnum().When(request => request.Type.HasValue);
    }
}