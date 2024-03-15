using FluentValidation;

namespace Saharaviewpoint.Core.Models.Input.User
{
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
                .MaximumLength(50).WithMessage("Maximum of 50 characters");

            RuleFor(model => model.LastName)
                .NotEmpty().WithMessage("Last name cannot be empty.")
                .MaximumLength(50).WithMessage("Maximum of 50 characters");

            RuleFor(model => model.Email)
                .EmailAddress();
        }
    }
}
