// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Models.Input;
using Saharaviewpoint.Models.Input.Project;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View.Project;

namespace Saharaviewpoint.API.Controllers;

[ApiController]
[Route("api/v1/approvals")]
public class ApprovalController(IApprovalService approvalService) : BaseController
{
    private readonly IApprovalService _approvalService = approvalService ?? throw new ArgumentNullException(nameof(approvalService));

    [HttpPost("projects/{projectId}/task-setups")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    public async Task<IActionResult> SendTaskSetupForApproval(int projectId)
    {
        var result = await _approvalService.SendTaskSetupForApproval(projectId);
        return ProcessResponse(result);
    }

    [HttpPost("projects/{projectId}/task-setups/{id}/send-reminder")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    public async Task<IActionResult> SendTaskSetupApprovalReminder(int projectId, int id)
    {
        var result = await _approvalService.SendTaskSetupApprovalReminder(projectId, id);
        return ProcessResponse(result);
    }

    [HttpGet("projects/{projectId}/task-setups")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<ProjectTaskApprovalView>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    public async Task<IActionResult> GetTaskSetupApproval(int projectId)
    {
        var result = await _approvalService.GetTaskSetupApproval(projectId);
        return ProcessResponse(result);
    }

    [HttpPost("projects/{projectId}/task-setups/{id}/approve")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<ProjectTaskApprovalView>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> ApproveTaskSetup(int projectId, int id, TaskApprovalModel model)
    {
        var result = await _approvalService.ApproveTaskSetup(projectId, id, model);
        return ProcessResponse(result);
    }

    [HttpGet("projects/task-setups")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<IEnumerable<ProjectTaskApprovalView>>))]
    public async Task<IActionResult> ListApprovalRequests([FromQuery] PagingOptionModel model)
    {
        var result = await _approvalService.ListApprovalRequests(model);
        return ProcessResponse(result);
    }
}
