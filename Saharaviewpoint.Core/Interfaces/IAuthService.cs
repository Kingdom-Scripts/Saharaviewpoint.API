// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Saharaviewpoint.Models.Input.Auth;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface IAuthService
{
    Task<Result> CreateClient(RegisterModel model);

    Task<Result> AuthenticateUser(LoginModel model);

    Task<Result> RefreshToken(RefreshTokenModel model);

    Task<Result> Logout(string userReference);

    Task<Result> UserProfile();
    Task<Result> ForgotPassword(ForgotPasswordModel model);
    Task<Result> ResetPassword(ResetPasswordModel model);
}