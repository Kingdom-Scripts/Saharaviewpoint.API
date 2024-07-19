// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using FluentValidation;

namespace Saharaviewpoint.Models.Input.Auth;

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