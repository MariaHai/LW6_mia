using FluentValidation;
using LW3.Dtos;

namespace LW3.Validators;

// Спільні правила для створення та редагування майстра
public abstract class MechanicDetailsValidator<T> : AbstractValidator<T> where T : MechanicDetailsDto
{
    protected MechanicDetailsValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100)
            .Matches(@"^[A-ZА-ЯІЇЄҐ][a-zа-яіїєґ'’\-]+(\s[A-ZА-ЯІЇЄҐ][a-zа-яіїєґ'’\-]+){1,2}$")
            .WithMessage("ПІБ: 2-3 слова, кожне з великої літери (напр. Петренко Іван Олегович).");

        RuleFor(x => x.Specialization).IsInEnum();

        RuleFor(x => x.HourlyRate)
            .GreaterThan(0)
            .LessThanOrEqualTo(100000);
    }
}

public class CreateMechanicDtoValidator : MechanicDetailsValidator<CreateMechanicDto>
{
}

public class UpdateMechanicDtoValidator : MechanicDetailsValidator<UpdateMechanicDto>
{
}
