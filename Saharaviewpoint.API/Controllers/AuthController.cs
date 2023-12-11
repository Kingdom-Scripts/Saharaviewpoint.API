using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.Input;
using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult))]
        public async Task<IActionResult> SignUp(RegisterModel model)
        {
            var res = _authService.CreateUser(model);
            return ProcessResponse(res);
        }

        //[HttpGet("{id}")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult))]
        //[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResult<List<string>>))]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ErrorResult))]
        //public async Task<IActionResult> GetAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    throw new NotImplementedException();
        //}
    }
}