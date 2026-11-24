using FluentValidation;
using LW3.Dtos;

namespace LW3.Validators;

// Спільні правила для створення та редагування автомобіля
public abstract class VehicleDetailsValidator<T> : AbstractValidator<T> where T : VehicleDetailsDto
{
    protected VehicleDetailsValidator()
    {
        RuleFor(x => x.LicensePlate)
            .NotEmpty().WithMessage("Держномер обов'язковий.")
            .Matches(@"^[ABCEHIKMOPTXАВСЕНІКМОРТХ]{2}\d{4}[ABCEHIKMOPTXАВСЕНІКМОРТХ]{2}$")
            .WithMessage("Держномер у форматі AA1234BB.");

        RuleFor(x => x.Make)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[\p{L}0-9\s\-]+$")
            .WithMessage("Марка може містити лише літери, цифри, пробіл і дефіс.");

        RuleFor(x => x.Model)
            .NotEmpty()
            .MaximumLength(50)
            .Matches(@"^[\p{L}0-9\s\-]+$")
            .WithMessage("Модель може містити лише літери, цифри, пробіл і дефіс.");

        RuleFor(x => x.Year)
            .InclusiveBetween(1980, DateTime.Now.Year + 1);

        RuleFor(x => x.Mileage)
            .InclusiveBetween(0, 5000000);

        RuleFor(x => x.OwnerPhone)
            .NotEmpty()
            .Matches(@"^\+380\d{9}$")
            .WithMessage("Телефон у форматі +380XXXXXXXXX.");
    }
}

public class CreateVehicleDtoValidator : VehicleDetailsValidator<CreateVehicleDto>
{
    public CreateVehicleDtoValidator()
    {
        RuleFor(x => x.Vin)
            .NotEmpty().WithMessage("VIN обов'язковий.")
            .Matches(@"^[A-HJ-NPR-Z0-9]{17}$")
            .WithMessage("VIN: 17 символів (великі латинські літери та цифри, без I, O, Q).");
    }
}

public class UpdateVehicleDtoValidator : VehicleDetailsValidator<UpdateVehicleDto>
{
    public UpdateVehicleDtoValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
    }
}
