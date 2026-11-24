using FluentValidation;
using LW3.Dtos;
using MongoDB.Bson;

namespace LW3.Validators;

// Спільні правила для створення та редагування наряду
public abstract class RepairOrderDetailsValidator<T> : AbstractValidator<T> where T : RepairOrderDetailsDto
{
    protected RepairOrderDetailsValidator()
    {
        RuleFor(x => x.VehicleId)
            .NotEmpty()
            .Must(id => ObjectId.TryParse(id, out _))
            .WithMessage("VehicleId має бути коректним ідентифікатором (24 hex-символи).");

        RuleFor(x => x.MechanicId)
            .NotEmpty()
            .Must(id => ObjectId.TryParse(id, out _))
            .WithMessage("MechanicId має бути коректним ідентифікатором (24 hex-символи).");

        RuleFor(x => x.Complaints)
            .NotEmpty().WithMessage("Потрібна хоча б одна скарга клієнта.");

        RuleForEach(x => x.Complaints)
            .NotEmpty()
            .MaximumLength(200)
            .Matches(@"^[\p{L}\p{N}\s.,!?()'’\-]+$")
            .WithMessage("Скарга містить недопустимі символи.");
    }
}

public class UpdateRepairOrderDtoValidator : RepairOrderDetailsValidator<UpdateRepairOrderDto>
{
    public UpdateRepairOrderDtoValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.NormHours).InclusiveBetween(0, 500);
        RuleFor(x => x.PartsCost).GreaterThanOrEqualTo(0);
    }
}
