using FluentValidation;
using LW3.Dtos;

namespace LW3.Validators;

public abstract class PartDetailsValidator<T> : AbstractValidator<T> where T : PartDetailsDto
{
    protected PartDetailsValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Quantity).InclusiveBetween(0, 1000000);
        RuleFor(x => x.UnitPrice).GreaterThan(0).LessThanOrEqualTo(10000000);
    }
}

public class CreatePartDtoValidator : PartDetailsValidator<CreatePartDto>
{
}

public class UpdatePartDtoValidator : PartDetailsValidator<UpdatePartDto>
{
}
