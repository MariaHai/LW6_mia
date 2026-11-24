using FluentValidation;
using LW3.Dtos;
using MongoDB.Bson;

namespace LW3.Validators;

public class CompleteOrderDtoValidator : AbstractValidator<CompleteOrderDto>
{
    public CompleteOrderDtoValidator()
    {
        RuleFor(x => x.ShiftMechanicId)
            .NotEmpty()
            .Must(id => ObjectId.TryParse(id, out _))
            .WithMessage("ShiftMechanicId має бути коректним ідентифікатором (24 hex-символи).");

        RuleFor(x => x.PaidAmount).GreaterThanOrEqualTo(0);

        RuleFor(x => x.CurrentMileage)
            .GreaterThanOrEqualTo(0)
            .When(x => x.CurrentMileage.HasValue);
    }
}
