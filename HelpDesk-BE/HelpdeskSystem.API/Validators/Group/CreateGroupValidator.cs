using FluentValidation;
using HelpdeskSystem.Common.DTOs.Groups;

namespace HelpdeskSystem.API.Validators.Group;

public class CreateGroupValidator: AbstractValidator<CreateGroupDto>
{
    public CreateGroupValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(60).WithMessage("Name cannot exceed 60 characters.")
            .Must(s => s.Trim().Length > 0).WithMessage("Name cannot be only spaces.");
    }
}
