using FluentValidation;
using HelpdeskSystem.Common.DTOs.Categories;

namespace HelpdeskSystem.API.Validators.Category;

public class UpdateCategoryValidator: AbstractValidator<UpdateCategoryDto>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(60).WithMessage("Name cannot exceed 60 characters.")
            .Must(s => s.Trim().Length > 0).WithMessage("Name cannot be only spaces.");
    }
}
