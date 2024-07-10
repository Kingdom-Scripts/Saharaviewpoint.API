using FluentValidation;

namespace Saharaviewpoint.Models.Input.Auth;

public class RegisterModel
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
}

public class RegisterModelValidation : AbstractValidator<RegisterModel>
{
    public RegisterModelValidation()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Your first name is required.")
            .Length(2, 20).WithMessage("Your first name must be between 2 and 20 characters.");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Your last name is required.")
            .Length(2, 20).WithMessage("Your last name must be between 2 and 20 characters.");
        RuleFor(x => x.Email).EmailAddress();
        RuleFor(x => x.ConfirmPassword).Equal(p => p.Password);
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Your password cannot be empty")
            .MinimumLength(8).WithMessage("Your password length must be at least 8.")
            .MaximumLength(20).WithMessage("Your password length must not exceed 20.")
            .Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.")
            .Matches(@"[\!\?\*\.\#\$\(\)]+").WithMessage("Your password must contain at least one (!?#$ *.).");
    }
}