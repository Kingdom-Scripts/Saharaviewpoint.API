// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using FluentValidation;

namespace Saharaviewpoint.Models.Input.User;

public class ProjectManagerModel
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? Phone { get; set; }
}

public class ProjectManagerModelValidator : AbstractValidator<ProjectManagerModel>
{
    public ProjectManagerModelValidator()
    {
        RuleFor(model => model.FirstName)
            .NotEmpty().WithMessage("First name cannot be empty.")
            .Length(2, 20).WithMessage("First name must be between 2 and 20 characters.");

        RuleFor(model => model.LastName)
            .NotEmpty().WithMessage("Last name cannot be empty.")
            .Length(2, 20).WithMessage("Last name must be between 2 and 20 characters.");

        RuleFor(model => model.Email)
            .EmailAddress();
    }
}