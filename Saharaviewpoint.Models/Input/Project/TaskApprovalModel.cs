// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using FluentValidation;

namespace Saharaviewpoint.Models.Input.Project;

public class TaskApprovalModel
{
    public bool Status { get; set; }
    public string Remark { get; set; }
}

public class TaskApprovalValidator : AbstractValidator<TaskApprovalModel>
{
    public TaskApprovalValidator()
    {
        RuleFor(m => m.Remark)
            .NotEmpty().When(m => m.Status == false).WithMessage("Remark is required when rejecting task setup.");
    }
}
