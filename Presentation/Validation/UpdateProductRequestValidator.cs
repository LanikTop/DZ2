using Application.DTOs;
using FluentValidation;

namespace Presentation.Validation
{
    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequestDTO>
    {
        public UpdateProductRequestValidator()
        {
            RuleFor(request => request.Id).NotEmpty();
            RuleFor(request => request.NewName).MaximumLength(200);
            RuleFor(request => request.NewPrice).GreaterThanOrEqualTo(0);
            RuleFor(request => request.NewWeight).GreaterThan(0);
            RuleFor(request => request.NewType).IsInEnum();
            RuleFor(request => request.NewWarehouseNumber).GreaterThan(0);
        }
    }
}
