using FluentValidation;
using HelpdeskSystem.Common.DTOs.Auth;

namespace HelpdeskSystem.API.Validators.Auth;

public class LoginRequestValidator: AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .Must(email => email?.Trim().Length > 0)
                .WithMessage("Email cannot be only spaces.")
            .EmailAddress().WithMessage("Please enter a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
    }
}
