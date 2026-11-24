using FluentValidation;
using LW3.Dtos;
using MongoDB.Bson;

namespace LW3.Validators;

public class ReceiveVehicleDtoValidator : AbstractValidator<ReceiveVehicleDto>
{
    public ReceiveVehicleDtoValidator()
    {
        RuleFor(x => x.Vin)
            .NotEmpty().WithMessage("VIN обов'язковий.")
            .Matches(@"^[A-HJ-NPR-Z0-9]{17}$")
            .WithMessage("VIN: 17 символів (великі латинські літери та цифри, без I, O, Q).");

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

        RuleFor(x => x.Year).InclusiveBetween(1980, DateTime.Now.Year + 1);
        RuleFor(x => x.Mileage).InclusiveBetween(0, 5000000);

        RuleFor(x => x.OwnerPhone)
            .NotEmpty()
            .Matches(@"^\+380\d{9}$")
            .WithMessage("Телефон у форматі +380XXXXXXXXX.");

        RuleFor(x => x.Complaints)
            .NotEmpty().WithMessage("Потрібна хоча б одна скарга клієнта.");

        RuleForEach(x => x.Complaints)
            .NotEmpty()
            .MaximumLength(200)
            .Matches(@"^[\p{L}\p{N}\s.,!?()'’\-]+$")
            .WithMessage("Скарга містить недопустимі символи.");

        RuleFor(x => x.MechanicId)
            .NotEmpty()
            .Must(id => ObjectId.TryParse(id, out _))
            .WithMessage("MechanicId має бути коректним ідентифікатором (24 hex-символи).");

        RuleFor(x => x.Specialization).IsInEnum();
    }
}
