using FluentValidation;

namespace Saharaviewpoint.Core.Models.Input.Auth
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginModelValidation : AbstractValidator<LoginModel>
    {
        public LoginModelValidation()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
