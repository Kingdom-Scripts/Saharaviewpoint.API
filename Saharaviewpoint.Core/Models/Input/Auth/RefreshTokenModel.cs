using FluentValidation;

namespace Saharaviewpoint.Core.Models.Input.Auth
{
    public class RefreshTokenModel
    {
        public string Token { get; set; }
    }

    public class RefreshTokenModelValidation : AbstractValidator<RefreshTokenModel>
    {
        public RefreshTokenModelValidation()
        {
            RuleFor(x => x.Token).NotEmpty();
        }
    }
}
