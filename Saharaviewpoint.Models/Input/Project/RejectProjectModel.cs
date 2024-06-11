using FluentValidation;

namespace Saharaviewpoint.Models.Input.Project;

public class RejectProjectModel
{
    public int ProjectId { get; set; }
    public required string Reason { get; set; }
}

public class RejectProjectValidator : AbstractValidator<RejectProjectModel>
{
    public RejectProjectValidator()
    {
        RuleFor(x => x.ProjectId).GreaterThan(0).WithMessage("Provide a valid project id.");
        RuleFor(x => x.Reason).NotEmpty().WithMessage("Reason is required.");
    }
}