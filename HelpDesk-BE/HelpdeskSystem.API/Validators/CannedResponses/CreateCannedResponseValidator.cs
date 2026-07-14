using FluentValidation;
using HelpdeskSystem.Common.DTOs.CannedResponses;

namespace HelpdeskSystem.API.Validators.CannedResponses;

public class CreateCannedResponseValidator : AbstractValidator<CreateCannedResponseDto>
{
    public CreateCannedResponseValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");
        
        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Body is required.")
            .MaximumLength(2000).WithMessage("Body must not exceed 2000 characters.")
            .MinimumLength(5).WithMessage("Body must have at least 5 characters.");
    }
}
