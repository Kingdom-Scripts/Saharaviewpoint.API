// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using FluentValidation;

namespace Saharaviewpoint.Models.Input.Auth;

public class LoginModel
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class LoginModelValidation : AbstractValidator<LoginModel>
{
    public LoginModelValidation()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}