using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Models.Input.ProjectManager;
using Saharaviewpoint.Models.Input.User;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View.Auth;

namespace Saharaviewpoint.API.Controllers;

[ApiController]
[Route("api/v1/project-managers")]
public class ProjectManagersController(IProjectManagerService userService) : BaseController
{
    private readonly IProjectManagerService _userService = userService ?? throw new ArgumentNullException(nameof(userService));

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<List<UserView>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> ListProjectManagersAsync([FromQuery] ProjectManagerSearchModel request)
    {
        var res = await _userService.ListProjectManagers(request);
        return ProcessResponse(res);
    }

    [HttpPost("invite")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> InviteProjectManagerAsync(ProjectManagerModel model)
    {
        var res = await _userService.InviteProjectManager(model);
        return ProcessResponse(res);
    }

    [HttpPost("accept-invitation")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> AcceptInvitation(AcceptInvitationModel model)
    {
        var res = await _userService.AcceptInvitation(model);
        return ProcessResponse(res);
    }

    [HttpPatch("{userUid}/suspend")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> SuspendUser(string userUid)
    {
        var res = await _userService.SuspendUser(userUid);
        return ProcessResponse(res);
    }

    [HttpPatch("{userUid}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> ActivateUser(string userUid)
    {
        var res = await _userService.ActivateUser(userUid);
        return ProcessResponse(res);
    }

    [HttpGet("check-email")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<bool>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> CheckIfEmailExist(string email)
    {
        var res = await _userService.CheckIfEmailExist(email);
        return ProcessResponse(res);
    }
}