// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using FluentValidation;

namespace Saharaviewpoint.Models.Input.Project;

public class ProjectTypeModel
{
    public string Name { get; set; }
}

public class TaskModelValidation : AbstractValidator<ProjectTypeModel>
{
    public TaskModelValidation()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}