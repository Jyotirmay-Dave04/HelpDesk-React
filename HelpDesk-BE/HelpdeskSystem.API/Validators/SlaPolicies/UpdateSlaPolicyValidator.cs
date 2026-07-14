using FluentValidation;
using HelpdeskSystem.Common.DTOs.SlaPolicies;

namespace HelpdeskSystem.API.Validators.SlaPolicies;

public class UpdateSlaPolicyValidator : AbstractValidator<UpdateSlaPolicyDto>
{
    public UpdateSlaPolicyValidator()
    {
        RuleFor(x => x.HoursToResolve)
            .GreaterThan(0).WithMessage("Hours must be greater than zero.")
            .LessThanOrEqualTo(720).WithMessage("Hours cannot exceed 720 (30 days).");
    }
}
