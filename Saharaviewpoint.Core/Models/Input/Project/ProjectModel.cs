using FluentValidation;
using Microsoft.AspNetCore.Http;
using Saharaviewpoint.Core.Utilities;

namespace Saharaviewpoint.Core.Models.Input.Project;

public class ProjectModel
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public bool IsPriority { get; set; }
    public int? AssigneeId { get; set; }
    public required string SizeOfSite { get; set; }
    public decimal Budget { get; set; }
    public required string Location { get; set; }
    public required string Type { get; set; }
    public string? SurroundingFacilities { get; set; }
    public IFormFile? Design { get; set; }
}

public class ProjectModelValidator : AbstractValidator<ProjectModel>
{
    public ProjectModelValidator()
    {
        RuleFor(model => model.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(50).WithMessage("Title cannot exceed 50 characters");

        RuleFor(model => model.Description)
            .MaximumLength(5000).WithMessage("Description cannot exceed 1000 characters");

        RuleFor(model => model.SizeOfSite)
            .NotEmpty().WithMessage("What is the size of this site?");

        RuleFor(model => model.StartDate)
            .NotEmpty().WithMessage("When is this project likely to start?")
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Due date must be in the future");

        RuleFor(model => model.DueDate)
            .NotEmpty().WithMessage("Due date is required")
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Due date must be in the future");

        RuleFor(model => model.Budget)
            .NotNull().WithMessage("Budget cannot be empty")
            .GreaterThan(0).WithMessage("Budget must be greater than 0");

        RuleFor(model => model.Location)
            .NotEmpty().WithMessage("Location is required");

        RuleFor(model => model.Type)
            .NotNull().WithMessage("Project type is required")
            .MinimumLength(2).WithMessage("Provide a valid project type of at least two characters");

        RuleFor(model => model.SurroundingFacilities)
            .MaximumLength(500).WithMessage("Surrounding Facilities cannot be more than 500 characters");

        RuleFor(model => model.Design)
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
