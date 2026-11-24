using FluentValidation;
using LW3.Dtos;

namespace LW3.Validators;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email обов'язковий.")
            .MaximumLength(100)
            .EmailAddress().WithMessage("Некоректний формат email.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Пароль обов'язковий.")
            .MinimumLength(8).WithMessage("Пароль має містити щонайменше 8 символів.")
            .MaximumLength(72).WithMessage("Пароль має містити не більше 72 символів.")
            .Matches("[A-Za-z]").WithMessage("Пароль має містити хоча б одну латинську літеру.")
            .Matches("[0-9]").WithMessage("Пароль має містити хоча б одну цифру.");
    }
}

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Email обов'язковий.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Пароль обов'язковий.");
    }
}

public class RefreshTokenDtoValidator : AbstractValidator<RefreshTokenDto>
{
    public RefreshTokenDtoValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh token обов'язковий.");
    }
}
