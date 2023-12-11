using FluentValidation;

namespace Saharaviewpoint.Core.Models.Input;

public class RegisterModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}

public class RegisterModelValidation : AbstractValidator<RegisterModel>
{
    public RegisterModelValidation ()
    {
        RuleFor(x =>  x.Email).EmailAddress();
        RuleFor(x => x.Password).Length(8, 20);
        RuleFor(x => x.ConfirmPassword).Equal(p => p.Password);
    }
}