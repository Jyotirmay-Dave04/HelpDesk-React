using FluentValidation;
using HelpdeskSystem.Common.DTOs.Tickets;
using HelpdeskSystem.Common.Enums;

namespace HelpdeskSystem.API.Validators.Tickets;

public class UpdateTicketValidator: AbstractValidator<UpdateTicketDto>
{
    public UpdateTicketValidator()
    {
        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Priority is required.")
            .Must(p => Enum.TryParse<Priority>(p, true, out _))
            .WithMessage("Priority must be Low, Medium, or High.");
 
        RuleFor(x => x.ServiceDetails)
            .NotEmpty().WithMessage("Service details are required.")
            .Must(s => s?.Trim().Length > 0).WithMessage("Service details cannot be only spaces.")
            .MinimumLength(10).WithMessage("Service details must be at least 10 characters.")
            .MaximumLength(2000).WithMessage("Service details cannot exceed 2000 characters.");

    }
}
