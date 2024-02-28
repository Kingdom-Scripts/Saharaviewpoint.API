using FluentValidation;

namespace Saharaviewpoint.Core.Models.Input.Auth;

public class RefreshTokenModel
{
    public required string RefreshToken { get; set; }
}

public class RefreshTokenModelValidation : AbstractValidator<RefreshTokenModel>
{
    public RefreshTokenModelValidation()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}