using FluentValidation;

namespace Saharaviewpoint.Models.Input.Task;

public class TaskDueDateModel
{
    public DateTime DueDate { get; set; }
    public required string Reason { get; set; }
}

public class TaskDueDateValidator : AbstractValidator<TaskDueDateModel>
{
    public TaskDueDateValidator()
    {
        RuleFor(model => model.DueDate)
            .NotEmpty().WithMessage("Due date cannot be empty.")
            .GreaterThan(DateTime.Now).WithMessage("Due date must be in the future.");

        RuleFor(model => model.Reason)
            .NotEmpty().WithMessage("Reason cannot be empty.")
            .Length(3, 5000).WithMessage("Reason must be between 2 and 5000 characters.");
    }
}