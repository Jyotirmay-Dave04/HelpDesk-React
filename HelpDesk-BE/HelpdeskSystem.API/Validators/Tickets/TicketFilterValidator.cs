using FluentValidation;
using HelpdeskSystem.Common.DTOs.Tickets;
using HelpdeskSystem.Common.Enums;

namespace HelpdeskSystem.API.Validators.Tickets;

public class TicketFilterValidator: AbstractValidator<TicketFilterDto>
{
    public TicketFilterValidator()
    {
        RuleFor(t => t.Status)
            .Must(s => Enum.TryParse<TicketStatus>(s, true, out var _)).WithMessage("Status must be valid.")
            .When(t => t.Status is not null);
        
        RuleFor(t => t.Priority)
            .Must(p => Enum.TryParse<Priority>(p, true, out var _)).WithMessage("Priority must be valid.")
            .When(t => t.Priority is not null);

        RuleFor(t => t.GroupId)
            .GreaterThan(0).WithMessage("Group must be valid.")
            .When(t => t.GroupId is not null);

        RuleFor(t => t.Search)
            .Must(s => s?.Trim().Length > 0).WithMessage("Search can not be only spaces.")
            .When(t => t.Search is not null);
        
        RuleFor(t => t.CategoryId)
            .GreaterThan(0).WithMessage("Category must be valid.")
            .When(t => t.CategoryId is not null);
        
        RuleFor(t => t.AssignedTo)
            .GreaterThan(0).WithMessage("Assigned agent must be valid.")
            .When(t => t.AssignedTo is not null);
    }
}
