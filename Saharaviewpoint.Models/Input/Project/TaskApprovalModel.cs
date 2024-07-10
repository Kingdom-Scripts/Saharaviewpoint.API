using FluentValidation;

namespace Saharaviewpoint.Models.Input.Project;

public class TaskApprovalModel
{
    public bool Status { get; set; }
    public string? Remark { get; set; }
}

public class TaskApprovalValidator : AbstractValidator<TaskApprovalModel>
{
    public TaskApprovalValidator()
    {
        RuleFor(m => m.Remark)
            .NotEmpty().When(m => m.Status == false).WithMessage("Remark is required when rejecting task setup.");
    }
}
