using FluentValidation;
using HelpdeskSystem.Common.DTOs.SubCategories;

namespace HelpdeskSystem.API.Validators.SubCategory;

public class UpdateSubCategoryValidator: AbstractValidator<UpdateSubCategoryDto>
{
    public UpdateSubCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(60).WithMessage("Name cannot exceed 60 characters.")
            .Must(s => s.Trim().Length > 0).WithMessage("Name cannot be only spaces.");
    }
}
