using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Models.Input.Auth;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View.Auth;

namespace Saharaviewpoint.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
[Authorize]
public class AuthController(IAuthService authService) : BaseController
{
    private readonly IAuthService _authService = authService ?? throw new ArgumentNullException(nameof(authService));

    [HttpPost("sign-up")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResult<AuthDataView>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> SignUp(RegisterModel model)
    {
        var res = await _authService.CreateClient(model);
        return ProcessResponse(res);
    }

    [HttpPost("token")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<AuthDataView>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> AuthenticateUser(LoginModel model)
    {
        var res = await _authService.AuthenticateUser(model);
        return ProcessResponse(res);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<AuthDataView>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> RefreshToken(RefreshTokenModel model)
    {
        var res = await _authService.RefreshToken(model);
        return ProcessResponse(res);
    }

    [HttpPost("{userReference}/logout")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult))]
    public async Task<IActionResult> AuthenticateUserAsync([FromRoute] string userReference)
    {
        var res = await _authService.Logout(userReference);
        return ProcessResponse(res);
    }

    [HttpGet("profile")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<UserProfileView>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> UserProfile()
    {
        var res = await _authService.UserProfile();
        return ProcessResponse(res);
    }

    /// <summary>
    /// Request password reset email
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> RequestPasswordReset(ForgotPasswordModel model)
    {
        var res = await _authService.ForgotPassword(model);
        return ProcessResponse(res);
    }

    /// <summary>
    /// Reset password
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<AuthDataView>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
    {
        var res = await _authService.ResetPassword(model);
        return ProcessResponse(res);
    }
}