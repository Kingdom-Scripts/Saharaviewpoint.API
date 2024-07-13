using FluentValidation;

namespace Saharaviewpoint.Models.Input.Project;

public class RejectProjectModel
{
    public required string Reason { get; set; }
}

public class RejectProjectValidator : AbstractValidator<RejectProjectModel>
{
    public RejectProjectValidator()
    {
        RuleFor(x => x.Reason).NotEmpty().WithMessage("Reason is required.");
    }
}