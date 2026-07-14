using FluentValidation;
using HelpdeskSystem.Common.DTOs.Comments;

namespace HelpdeskSystem.API.Validators.Comments;

public class CreateCommentValidator: AbstractValidator<CreateCommentDto>
{
    public CreateCommentValidator()
    {
        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Comment cannot be empty.")
            .MinimumLength(5).WithMessage("Comment must be at least 5 characters.")
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.");
        
        RuleFor(x => x.TicketId)
            .GreaterThan(0).WithMessage("Invalid Ticket.");
    }
}
