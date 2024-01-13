using FluentValidation;
using Saharaviewpoint.Core.Utilities;

namespace Saharaviewpoint.Core.Models.Input.Project;

public class ProjectStatusModel
{
    public string Status { get; set; }
}

public class ProjectStatusModelValidation : AbstractValidator<ProjectStatusModel>
{
    public ProjectStatusModelValidation()
    {
        RuleFor(x => x.Status)
            .NotNull()
            .WithMessage("Status is required")
            .Must(BeValidProjectStatus)
            .WithMessage($"Invalid status value. Must be one of the following: {ProjectStatuses.REQUESTED}, {ProjectStatuses.IN_PROGRESS}, {ProjectStatuses.COMPLETED}");
    }

    private bool BeValidProjectStatus(string status)
    {
        // Check if the provided status is one of the valid values
        return status == ProjectStatuses.REQUESTED ||
               status == ProjectStatuses.IN_PROGRESS ||
               status == ProjectStatuses.COMPLETED;
    }
}