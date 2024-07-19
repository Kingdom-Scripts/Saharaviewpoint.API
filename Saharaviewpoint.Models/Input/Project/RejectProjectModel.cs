// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

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