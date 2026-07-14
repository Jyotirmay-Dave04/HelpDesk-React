using FluentValidation;
using HelpdeskSystem.Common.DTOs.User;
using HelpdeskSystem.Common.Enums;

namespace HelpdeskSystem.API.Validators.User;

public class ChangeUserRolePayloadValidator: AbstractValidator<ChangeUserRolePayload>
{
    public ChangeUserRolePayloadValidator()
    {
        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required.")
            .Must(r => Enum.TryParse<UserRole>(r, true, out UserRole _)).WithMessage("Role must be valid.");
    }
}
