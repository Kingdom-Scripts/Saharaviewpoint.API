using FluentValidation;

namespace Saharaviewpoint.Core.Models.Input.Project;

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