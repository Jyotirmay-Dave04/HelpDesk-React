using FluentValidation;
using HelpdeskSystem.Common.DTOs.Categories;

namespace HelpdeskSystem.API.Validators.Category;

public class CreateCategoryValidator: AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(60).WithMessage("Name cannot exceed 60 characters.")
            .Must(s => s.Trim().Length > 0).WithMessage("Name cannot be only spaces.");
        
        RuleFor(x => x.GroupId)
            .GreaterThan(0).WithMessage("Group is required.");
    }
}
