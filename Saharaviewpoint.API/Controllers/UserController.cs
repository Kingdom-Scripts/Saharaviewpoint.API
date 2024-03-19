using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.Input.User;
using Saharaviewpoint.Core.Models.Utilities;
using Saharaviewpoint.Core.Models.View.Auth;

namespace Saharaviewpoint.API.Controllers
{
    [ApiController]
    [Route("api/v1/users")]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [HttpGet("project-managers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<List<UserView>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
        public async Task<IActionResult> ListProjectManagersAsync(int pageIndex, int pageSize)
        {
            var res = await _userService.ListProjectManagersAsync(pageIndex, pageSize);
            return ProcessResponse(res);
        }

        [HttpPost("project-managers/invite")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
        public async Task<IActionResult> InviteProjectManagerAsync(ProjectManagerModel model)
        {
            var res = await _userService.InviteProjectManagerAsync(model);
            return ProcessResponse(res);
        }

        [HttpPost("accept-invitation")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
        public async Task<IActionResult> AcceptInvitation(AcceptInvitationModel model)
        {
            var res = await _userService.AcceptInvitation(model);
            return ProcessResponse(res);
        }
    }
}
