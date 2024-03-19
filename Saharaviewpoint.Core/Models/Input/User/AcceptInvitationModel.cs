using FluentValidation;

namespace Saharaviewpoint.Core.Models.Input.User
{
    public class AcceptInvitationModel
    {
        public required string Email { get; set; }
        public required string Token { get; set; }
        public required string Type { get; set; }
                public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class AcceptInvitationModelValidator : AbstractValidator<AcceptInvitationModel>
    {
        public AcceptInvitationModelValidator()
        {
            RuleFor(model => model.Email)
                .EmailAddress();

            RuleFor(model => model.Token)
                .NotEmpty().WithMessage("Invalid invitation request.");

            RuleFor(model => model.Type)
                .NotEmpty().WithMessage("Invalid invitation request.");

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
}
