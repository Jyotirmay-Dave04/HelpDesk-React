using FluentValidation;
using HelpdeskSystem.Common.DTOs.Tickets;
using HelpdeskSystem.Common.Enums;

namespace HelpdeskSystem.API.Validators.Tickets;

public class UpdateStatusValidator: AbstractValidator<UpdateStatusDto>
{
    public UpdateStatusValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => Enum.TryParse<TicketStatus>(s, true, out _))
            .WithMessage("Status must be valid.");
        
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("A reason is required for this status change.")
            .When(x => Enum.Parse<TicketStatus>(x.Status) == TicketStatus.OnHold || Enum.Parse<TicketStatus>(x.Status) == TicketStatus.CannotResolve);
        
        RuleFor(x => x.Reason)
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.")
            .When(x => x.Reason != null);
    }
}
