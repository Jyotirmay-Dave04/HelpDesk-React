using FluentValidation;
using HelpdeskSystem.Common.DTOs.User;

namespace HelpdeskSystem.API.Validators.User;

public class AssignAgentRequestDtoValidator: AbstractValidator<AssignAgentRequestDto>
{
    public AssignAgentRequestDtoValidator()
    {
        RuleFor(x => x.Search)
            .Must(s => s?.Trim().Length > 0).WithMessage("Search can not be only spaces.")
            .When(x => x.Search is not null);
    }
}
