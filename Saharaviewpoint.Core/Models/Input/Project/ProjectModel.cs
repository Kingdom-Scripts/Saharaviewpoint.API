using FluentValidation;
using Microsoft.AspNetCore.Http;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.App.Constants;
using Saharaviewpoint.Core.Utilities;

namespace Saharaviewpoint.Core.Models.Input.Project;

public class ProjectModel
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsPriority { get; set; }
    public int? AssigneeId { get; set; }
    public string SizeOfSite { get; set; }
    public decimal Budget { get; set; }
    public string Location { get; set; }
    public int TypeId { get; set; }
    public string SiteCondition { get; set; }
    public IFormFile Design { get; set; }
}

public class ProjectModelValidator : AbstractValidator<ProjectModel>
{
    public ProjectModelValidator()
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");

        RuleFor(model => model.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(model => model.DueDate)
            .NotEmpty().WithMessage("Due date is required")
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Due date must be in the future");

        RuleFor(model => model.Budget)
            .GreaterThan(0).WithMessage("Budget must be greater than 0");

        RuleFor(model => model.Location)
            .NotEmpty().WithMessage("Location is required");

        RuleFor(model => model.TypeId)
            .GreaterThan(0).WithMessage("Type is required");

        RuleFor(model => model.Design)
            .Cascade(CascadeMode.Stop)  // Stop validation if Design is null
            .NotNull().WithMessage("Design file is required.")
            .When(model => model.Design != null)  // Apply validation only when Design is not null
            .Custom((design, context) =>
            {
                var validationResult = CustomFileValidator.HaveValidFile(design);
                if (!validationResult.IsValid)
                {
                    context.AddFailure($"Design: {validationResult.ErrorMessage}");
                }
            });
    }
}
