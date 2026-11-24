using FluentValidation;
using LW3.Dtos;
using MongoDB.Bson;

namespace LW3.Validators;

public class ApproveEstimateDtoValidator : AbstractValidator<ApproveEstimateDto>
{
    public ApproveEstimateDtoValidator()
    {
        RuleFor(x => x.NormHours)
            .GreaterThan(0).WithMessage("Кількість нормогодин має бути більшою за 0.")
            .LessThanOrEqualTo(500);

        RuleFor(x => x.AdvanceAmount).GreaterThanOrEqualTo(0);

        RuleForEach(x => x.Parts).ChildRules(part =>
        {
            part.RuleFor(p => p.PartId)
                .NotEmpty()
                .Must(id => ObjectId.TryParse(id, out _))
                .WithMessage("PartId має бути коректним ідентифікатором (24 hex-символи).");

            part.RuleFor(p => p.Quantity)
                .GreaterThan(0).WithMessage("Кількість запчастини має бути більшою за 0.");
        });

        RuleFor(x => x.Parts)
            .Must(parts => parts.Select(p => p.PartId).Distinct().Count() == parts.Count)
            .WithMessage("Одна й та сама запчастина не може повторюватись у списку.");
    }
}
