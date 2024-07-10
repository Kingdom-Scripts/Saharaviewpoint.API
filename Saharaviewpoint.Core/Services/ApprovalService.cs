using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Saharaviewpoint.Models.App.Constants;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.Input.Project;
using Saharaviewpoint.Models.Input;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View.Project;
using Saharaviewpoint.Models.Input.Auth;
using Saharaviewpoint.Core.Extensions;
using Mapster;
using Saharaviewpoint.Core.Interfaces;
using Microsoft.AspNetCore.Diagnostics;

namespace Saharaviewpoint.Core.Services;

public class ApprovalService(SaharaviewpointContext context, UserSession userSession) : IApprovalService
{

    private readonly SaharaviewpointContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly UserSession _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));

    public async Task<Result> SendTaskSetupForApproval(int projectId)
    {
        // get currently opened approval
        bool approvalExist = await _context.ProjectTaskApprovals
            .AnyAsync(pta => pta.ProjectId == projectId && !pta.IsFulfilled);

        if (approvalExist)
            return new ErrorResult("Task setup approval request already exists.");

        // validate the project still exists
        bool projectExist = await _context.Projects
            .AnyAsync(p => p.Id == projectId && !p.IsDeleted);

        if (!projectExist)
            return new BadErrorResult("Project does not exist");


        var approval = new ProjectTaskApproval
        {
            ProjectId = projectId,
            RequesterId = _userSession.UserId
        };

        await _context.ProjectTaskApprovals.AddAsync(approval);

        // add log
        var log = new ProjectLog
        {
            ProjectId = projectId,
            Type = ProjectLogTypes.Approvals.ToString(),
            Description = $"Task setup sent for approval by {_userSession.Name}",
            CreatedById = _userSession.UserId
        };

        await _context.ProjectLogs.AddAsync(log);

        // TODO:  send reminder email 

        // save the changes
        int saved = await _context.SaveChangesAsync();

        // TODO: send an email notifying the client and admin about the request.

        return saved > 0
            ? new SuccessResult(approval.Adapt<ProjectTaskApprovalView>())
            : new ErrorResult("Unable to save changes, please try again later.");
    }

    public async Task<Result> SendTaskSetupApprovalReminder(int projectId, int id)
    {
        var approval = await _context.ProjectTaskApprovals
            .FirstOrDefaultAsync(pta => pta.Id == id && pta.ProjectId == projectId && !pta.IsFulfilled);

        if (approval is null)
            return new ErrorResult("Invalid approval, request has not been initiated.");

        if (approval.IsFulfilled)
            return new ErrorResult("Task setup approval request has been fulfilled.");

        // add log
        var log = new ProjectLog
        {
            Type = ProjectLogTypes.Approvals.ToString(),
            Description = $"Reminder sent for task setup approval by {_userSession.Name}",
            ProjectId = projectId,
            CreatedById = _userSession.UserId
        };
        await _context.ProjectLogs.AddAsync(log);

        // save the changes
        int saved = await _context.SaveChangesAsync();

        return new SuccessResult("Reminder sent successfuly.");
    }

    public async Task<Result> GetTaskSetupApproval(int projectId)
    {
        var approval = await _context.ProjectTaskApprovals
            .Where(pta => pta.ProjectId == projectId)
            .OrderByDescending(pta => pta.CreatedAt)
            .ProjectToType<ProjectTaskApprovalView>()
            .LastOrDefaultAsync();

        return approval is not null
            ? new SuccessResult(approval)
            : new SuccessResult(status: StatusCodes.Status204NoContent, message: "No task setup approval request found.");
    }

    public async Task<Result> ApproveTaskSetup(int projectId, int id, TaskApprovalModel model)
    {
        // get currently opened approval
        var approval = await _context.ProjectTaskApprovals
            .FirstOrDefaultAsync(pta => pta.Id == id && pta.ProjectId == projectId && !pta.IsFulfilled);

        if (approval is null)
            return new ErrorResult(StatusCodes.Status404NotFound, "Invalid approval, not found.");

        if (approval.IsFulfilled)
            return new ErrorResult("Task setup approval request has been fulfilled.");

        var project = await _context.Projects
            .Where(p => p.Id == projectId)
            .Select(p => new { p.Id, p.IsDeleted })
            .FirstAsync();

        if (project.IsDeleted)
            return new BadErrorResult("Project has been deleted, you cannot act on this project anymore.");

        var user = await _context.Users.FindAsync(_userSession.UserId);
        approval.IsFulfilled = true;
        approval.FulfilledBy = user;
        approval.FulfilledOn = DateTime.UtcNow;
        approval.Status = model.Status;
        approval.Remark = model.Remark;

        // TODO: send an email notifying the PM and Client about the approval

        // add log
        string logMessage = model.Status
            ? $"Task setup approved by {_userSession.Name}"
            : $"Task setup approval request declined by {_userSession.Name}";
        var log = new ProjectLog
        {
            Type = ProjectLogTypes.Approvals.ToString(),
            Description = logMessage,
            ProjectId = project.Id,
            CreatedById = _userSession.UserId
        };
        await _context.ProjectLogs.AddAsync(log);

        // save the changes
        int saved = await _context.SaveChangesAsync();

        string message = model.Status
            ? "Task setup approved successfully."
            : "Task setup approval request declined.";

        return saved > 0
        ? new SuccessResult(message, approval.Adapt<ProjectTaskApprovalView>())
            : new ErrorResult("Unable to save changes, please try again later.");
    }

    public async Task<Result> ListApprovalRequests(PagingOptionModel model)
    {
        if (!string.IsNullOrEmpty(model.SearchQuery))
            model.SearchQuery = model.SearchQuery.ToLower().Trim();

        var approvalsQuery = _context.ProjectTaskApprovals
            .AsQueryable();

        if (!string.IsNullOrEmpty(model.SearchQuery))
        {
            string searchLike = $"%{model.SearchQuery}%";
            approvalsQuery = approvalsQuery.Where(pta =>
                EF.Functions.Like(pta.Project!.Title, searchLike) ||
                EF.Functions.Like(pta.Requester!.FirstName, searchLike) ||
                EF.Functions.Like(pta.Requester.LastName, searchLike) ||
                EF.Functions.Like(pta.Project!.CreatedBy!.FirstName, searchLike) ||
                EF.Functions.Like(pta.Project.CreatedBy.LastName, searchLike));
        }

        var approvals = await approvalsQuery
            .OrderByDescending(pta => pta.CreatedAt)
            .ProjectToType<ProjectTaskApprovalView>()
            .ToPaginatedListAsync(model.PageIndex, model.PageSize);

        return new SuccessResult(approvals);
    }
}
