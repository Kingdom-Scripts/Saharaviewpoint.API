using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.Input.Auth;
using Saharaviewpoint.Core.Models.Utilities;
using Saharaviewpoint.Core.Models.View.Auth;

namespace Saharaviewpoint.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    [Authorize]
    public class AuthController : BaseController
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;

        public AuthController(ILogger<AuthController> logger, SaharaviewpointContext context, IAuthService authService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost("sign-up")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResult<AuthDataView>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
        public async Task<IActionResult> SignUp(RegisterModel model)
        {
            var res = await _authService.CreateUserAsync(model);
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
    }
}