// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using FluentValidation;

namespace Saharaviewpoint.Models.Input.Task;

public class TaskStatusModel
{
    public required string Status { get; set; }
    public string? Reason { get; set; }
}

public class TaskStatusValidator : AbstractValidator<TaskStatusModel>
{
    public TaskStatusValidator()
    {
        RuleFor(m => m.Status)
            .NotEmpty()
            .WithMessage("Status is required");

        RuleFor(m => m.Reason)
            .MaximumLength(5000)
            .WithMessage("Reason cannot exceed 5000 characters");
    }
}