using FluentValidation;
using HelpdeskSystem.Common.DTOs.Auth;

namespace HelpdeskSystem.API.Validators.Auth;

public class RegisterRequestValidator: AbstractValidator<RegisterDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MinimumLength(2).WithMessage("Name must be at least 2 characters.")
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
            // noWhitespaceValidator — trim check
            .Must(name => name?.Trim().Length > 0)
                .WithMessage("Name cannot be only spaces.")
            // userNameValidator — must start with letter or underscore, no special chars
            .Matches(@"^[A-Za-z_][A-Za-z0-9_ ]*$")
                .WithMessage("Name must start with a letter or underscore and contain no special characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .MaximumLength(100).WithMessage("Email cannot exceed 100 characters.")
            // noWhitespaceValidator
            .Must(email => email?.Trim().Length > 0)
                .WithMessage("Email cannot be only spaces.")
            .EmailAddress().WithMessage("Please enter a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            // passwordStrengthValidator — all four checks together
            .Must(password =>
            {
                bool hasUpper   = password.Any(char.IsUpper);
                bool hasLower   = password.Any(char.IsLower);
                bool hasDigit   = password.Any(char.IsDigit);
                bool hasSpecial = password.Any(c => "!@#$%^&*()_+-=[]{};\':\"\\|,.<>/?".Contains(c));
                return hasUpper && hasLower && hasDigit && hasSpecial;
            })
            .WithMessage("Password must contain uppercase, lowercase, number and special character (!@#$%^&* etc).");
    }
}
