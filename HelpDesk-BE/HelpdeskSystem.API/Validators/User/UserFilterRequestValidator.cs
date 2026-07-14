using FluentValidation;
using HelpdeskSystem.Common.DTOs.User;
using HelpdeskSystem.Common.Enums;

namespace HelpdeskSystem.API.Validators.User;

public class UserFilterRequestValidator: AbstractValidator<UserFilterRequest>
{
    public UserFilterRequestValidator()
    {
        RuleFor(x => x.Role)
            .Must(r => Enum.TryParse<UserRole>(r, true, out var _)).WithMessage("Role must be valid.")
            .When(x => x.Role is not null);
        
        RuleFor(x => x.Search)
            .Must(s => s?.Trim().Length > 0).WithMessage("Search can not be only spaces.")
            .When(x => x.Search is not null);
    }
}
