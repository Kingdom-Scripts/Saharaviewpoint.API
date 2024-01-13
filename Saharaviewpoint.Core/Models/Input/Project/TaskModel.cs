using FluentValidation;

namespace Saharaviewpoint.Core.Models.Input.Project;

public class TaskModel
{
    public string Name { get; set; }
}

public class TaskModelValidation : AbstractValidator<TaskModel>
{
    public TaskModelValidation()
    {
        RuleFor(x => x.Name)
            .NotNull();
    }
}