// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using FluentValidation;
using Microsoft.AspNetCore.Http;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Models.Input.Task;

public class TaskModel
{
    public int ProjectId { get; set; }
    public int ParentId { get; set; }
    public required string Type { get; set; }
    public required string Summary { get; set; }
    public string Description { get; set; }
    public DateTime ExpectedStartDate { get; set; }
    public DateTime? DueDate { get; set; }

    public List<IFormFile> Attachments { get; set; } = new();
}

public class TaskModelValidator : AbstractValidator<TaskModel>
{
    public TaskModelValidator()
    {
        RuleFor(model => model.ProjectId)
            .NotEmpty().WithMessage("Project is required");

        // ParentId should be required if Type is SubTask
        RuleFor(model => model.ParentId)
            .GreaterThan(0).When(model => model.Type == "SubTask").WithMessage("Which task does this task belong to?");

        RuleFor(model => model.Type)
            .NotEmpty().WithMessage("Type is required")
            .MinimumLength(2).WithMessage("Type must be at least two characters");

        RuleFor(model => model.Summary)
            .NotEmpty().WithMessage("Summary is required")
            .MaximumLength(200).WithMessage("Summary cannot exceed 200 characters");

        RuleFor(model => model.Description)
            .MaximumLength(5000).WithMessage("Description cannot exceed 5000 characters");

        RuleFor(model => model.ExpectedStartDate)
            .NotEmpty().WithMessage("Expected start date is required")
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Expected start date must be in the future");

        RuleFor(model => model.DueDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow).WithMessage("Due date must be in the future")
            // add a rule that it must be in the future of start date
            .GreaterThanOrEqualTo(model => model.DueDate).WithMessage("Due date must be in the future of start date");

        RuleForEach(model => model.Attachments)
            .Custom((file, context) =>
            {
                var validationResult = CustomFileValidator.HaveValidFile(file);
                if (!validationResult.IsValid)
                {
                    context.AddFailure($"File: {validationResult.ErrorMessage}");
                }
            });
    }
}