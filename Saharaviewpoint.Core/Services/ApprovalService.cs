// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using LazyCache;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Saharaviewpoint.Core.Contants.CacheKeys;
using Saharaviewpoint.Core.Extensions;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Utilities;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.App.Constants;
using Saharaviewpoint.Models.Configurations;
using Saharaviewpoint.Models.Email;
using Saharaviewpoint.Models.Input;
using Saharaviewpoint.Models.Input.Auth;
using Saharaviewpoint.Models.Input.Project;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View.Project;

namespace Saharaviewpoint.Core.Services;

public class ApprovalService(SaharaviewpointContext context, UserSession userSession, IAppCache cache, IEmailService emailService,
        IOptions<AppConfig> options) : BaseService, IApprovalService
{

    private readonly SaharaviewpointContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly UserSession _userSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
    private readonly IAppCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));
    private readonly IEmailService _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    private readonly BaseUrLs _baseUrls = options.Value.BaseUrLs;

    public async Task<Result> SendTaskSetupForApproval(int projectId)
    {
        // get currently opened approval
        bool approvalExist = await _context.ProjectTaskApprovals
            .AnyAsync(pta => pta.ProjectId == projectId && !pta.IsFulfilled);

        if (approvalExist)
            return new ErrorResult("Task setup approval request already exists.");

        // validate the project still exists
        var project = await _context.Projects
            .Where(p => p.Id == projectId && !p.IsDeleted)
            .Select(p => new
            {
                p.Id,
                p.Title,
                OwnerFirstName = p.CreatedBy!.FirstName,
                OwnerLastName = p.CreatedBy.LastName,
                OwnerEmail = p.CreatedBy.Email
            }).FirstOrDefaultAsync();

        if (project is null)
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

        // save the changes
        int saved = await _context.SaveChangesAsync();

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        _cache.ClearCaches(CacheKeys.TaskApprovalRequest(), CacheKeys.TaskSetupApproval(projectId));

        // Send Notification Email
        {
            var initiator = await _context.Users
                .Where(u => u.Id == _userSession.UserId)
                .Select(u => new
                {
                    Name = $"{u.FirstName} {u.LastName}",
                }).FirstAsync();

            string url = $"{_baseUrls.Admin}/approvals/project-task-setup";

            var adminEmailRequest = new GenericEmailModel
            {
                To = _emailService.GetUserEmails(RolesConstants.SvpAdmin, RolesConstants.SuperAdmin),
                Subject = $"Task Approval Request - {project.Title}",
                Salutation = "Hello,",
                PrimaryMessage = "A task setup approval request has been initiated by a project manager. Kindly review the request and take necessary action.<br><br>" +
                    "<strong><span style=\"font-size:larger;\">Request Details</span></strong><br>" +
                    $"<strong>Project:</strong> {project.Title}<br>" +
                    $"<strong>Project Owner:</strong> {project.OwnerFirstName} {project.OwnerLastName}<br>" +
                    $"<strong>Initiator:</strong> {initiator.Name}<br>" +
                    $"<strong>Date Initiated:</strong> {DateTime.UtcNow:dd MMM, yyyy}<br>",
                ClosingRemark = "Regards",
                ActionButton = new()
                {
                    Text = "View Pending Requests",
                    Url = url
                }
            };

            await _emailService.SendEmail(adminEmailRequest);

            var clientEmailRequest = new GenericEmailModel
            {
                To = [new EmailAddress{Address = project.OwnerEmail, Name = $"{project.OwnerFirstName} {project.OwnerLastName}"}],
                Subject = $"{project.Title} - Project Update",
                Salutation = $"Hello {project.OwnerFirstName},",
                PrimaryMessage = $"This is to notify you that your project - {project.Title} - has been completely setup and sent for approval by the assigned project manager. An administrator will review and address the request soon.<br><br>" +
                    "<strong><span style=\"font-size:larger;\">Details</span></strong><br>" +
                    $"<strong>Project:</strong> {project.Title}<br>" +
                    $"<strong>Project Manager:</strong> {initiator.Name}<br>" +
                    $"<strong>Date Initiated:</strong> {DateTime.UtcNow:dd MMM, yyyy}<br>",
                ClosingRemark = "Regards",
                ActionButton = new EmailActionButton
                {
                    Text = "View Project",
                    Url = $"{_baseUrls.Client}/project/details/{projectId}"
                }
            };
            await _emailService.SendEmail(clientEmailRequest);
        }

        return new SuccessResult(approval.Adapt<ProjectTaskApprovalView>());
    }

    public async Task<Result> SendTaskSetupApprovalReminder(int projectId, int id)
    {
        var approval = await _context.ProjectTaskApprovals
            .Where(pta => pta.Id == id && pta.ProjectId == projectId)
            .Select(pta => new
            {
                pta.IsFulfilled,
                InitiatorName = $"{pta.Requester!.FirstName} {pta.Requester.LastName}",
                Project = new
                {
                    pta.Project!.Title,
                    OwnerFirstName = pta.Project.CreatedBy!.FirstName,
                    OwnerLastName = pta.Project.CreatedBy.LastName
                },
            })
            .FirstOrDefaultAsync();

        if (approval is null)
            return new ErrorResult("Invalid approval, request has not been initiated.");

        if (approval.IsFulfilled)
            return new ErrorResult("Task setup approval request has been fulfilled.");

        string url = $"{_baseUrls.Admin}/approvals/project-task-setup";

        var adminEmailRequest = new GenericEmailModel
        {
            To = _emailService.GetUserEmails(RolesConstants.SvpAdmin, RolesConstants.SuperAdmin),
            Subject = $"(Reminder) Task Approval Request - {approval.Project.Title}",
            Salutation = "Hello,",
            PrimaryMessage = "A task setup approval request has been initiated by a project manager. Kindly review the request and take necessary action.<br><br>" +
                             "<strong><span style=\"font-size:larger;\">Request Details</span></strong><br>" +
                             $"<strong>Project:</strong> {approval.Project.Title}<br>" +
                             $"<strong>Project Owner:</strong> {approval.Project.OwnerFirstName} {approval.Project.OwnerLastName}<br>" +
                             $"<strong>Initiator:</strong> {approval.InitiatorName}<br>" +
                             $"<strong>Date Initiated:</strong> {DateTime.UtcNow:dd MMM, yyyy}<br>",
            ClosingRemark = "Regards",
            ActionButton = new()
            {
                Text = "View Pending Requests",
                Url = url
            }
        };

        await _emailService.SendEmail(adminEmailRequest);

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
        await _context.SaveChangesAsync();

        return new SuccessResult("Reminder sent successfuly.");
    }

    public async Task<Result> GetTaskSetupApproval(int projectId)
    {
        var approval = await _cache.GetOrAddAsync(CacheKeys.TaskSetupApproval(projectId), async () =>
        {
            return await _context.ProjectTaskApprovals
                .Where(pta => pta.ProjectId == projectId)
                .OrderByDescending(pta => pta.CreatedAt)
                .ProjectToType<ProjectTaskApprovalView>()
                .LastOrDefaultAsync();
        }, new TimeSpan(0, 45, 0));

        return approval is not null
            ? new SuccessResult(approval)
            : new SuccessResult(status: StatusCodes.Status204NoContent, message: "No task setup approval request found.");
    }

    public async Task<Result> ApproveTaskSetup(int projectId, int id, TaskApprovalModel model)
    {
        // get currently opened approval
        var approval = await _context.ProjectTaskApprovals
            .Include(a => a.Requester)
            .FirstOrDefaultAsync(pta => pta.Id == id && pta.ProjectId == projectId && !pta.IsFulfilled);

        if (approval is null)
            return new ErrorResult(StatusCodes.Status404NotFound, "Invalid approval, not found.");

        if (approval.IsFulfilled)
            return new ErrorResult("Task setup approval request has been fulfilled.");

        var project = await _context.Projects
            .Where(p => p.Id == projectId)
            .Select(p => new { p.Id, p.IsDeleted, p.Title,
                OwnerEmail = p.CreatedBy.Email,
                OwnerFirstName = p.CreatedBy.FirstName, OwnerLastName = p.CreatedBy.LastName })
            .FirstAsync();

        if (project.IsDeleted)
            return new BadErrorResult("Project has been deleted, you cannot act on this project anymore.");

        var user = await _context.Users.FindAsync(_userSession.UserId);
        approval.IsFulfilled = true;
        approval.FulfilledBy = user;
        approval.FulfilledOn = DateTime.UtcNow;
        approval.Status = model.Status;
        approval.Remark = model.Remark;

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

        if (saved < 1)
            return new ErrorResult("Unable to save changes, please try again later.");

        string details = "<strong><span style=\"font-size:larger;\">Details</span></strong><br>" +
                         $"<strong>Project:</strong> {project.Title}<br>" +
                         $"<strong>Approval Status:</strong> {(model.Status ? "Approved" : "Declined")}<br>" +
                         $"<strong>Reviewed By:</strong> {_userSession.Name}<br>" +
                         $"<strong>Date Reviewed:</strong> {approval.FulfilledOn:dd MMM, yyyy}<br>" +
                         $"{(!string.IsNullOrEmpty(model.Remark) ? $"<strong>Remark:</strong> {model.Remark}<br>" : "")}";

        var emailToPm = new GenericEmailModel
        {
            To = [new EmailAddress{Address = approval.Requester!.Email, Name = $"{approval.Requester.FirstName} {approval.Requester.LastName}"}],
            Subject = $"Task Setup Approval - {project.Title}",
            Salutation = $"Hello {approval.Requester.FirstName},",
            PrimaryMessage = $"Your task setup approval request for project - {project.Title} " +
                             $"has been reviewed and {(model.Status ? "approved" : "declined")} by an administrator.<br><br>" + details,
            ClosingRemark = "Regards",
            ActionButton = new EmailActionButton
            {
                Text = "View Project",
                Url = $"{_baseUrls.Client}/tasks/all?projectId={projectId}"
            }
        };

        var emailToClient = new GenericEmailModel
        {
            To = [new EmailAddress{Address = project.OwnerEmail, Name = $"{project.OwnerFirstName} {project.OwnerLastName}"}],
            Subject = $"Task Setup Approval - {project.Title}",
            Salutation = $"Hello {project.OwnerFirstName},",
            PrimaryMessage = $"This is to notify you that the task setup for project - {project.Title} " +
                             $"has been reviewed and {(model.Status ? "approved" : "declined")} by an administrator.<br><br>" + details,
            ClosingRemark = "Regards",
            ActionButton = new EmailActionButton
            {
                Text = "View Project",
                Url = $"{_baseUrls.Client}/project/details/{projectId}"
            }
        };

        await _emailService.SendEmail(emailToPm);
        await _emailService.SendEmail(emailToClient);

        string message = model.Status
            ? "Task setup approved successfully."
            : "Task setup approval request declined.";

        _cache.ClearCaches(
            CacheKeys.TaskApprovalRequest(),
            CacheKeys.TaskSetupApproval(projectId),
            CacheKeys.TaskListCacheKeys(),
            CacheKeys.BoardTasksValidation(projectId),
            CacheKeys.BoardTasks(projectId));

        return new SuccessResult(message, approval.Adapt<ProjectTaskApprovalView>());
    }

    public async Task<Result> ListApprovalRequests(PagingOptionModel request)
    {
        string generatedKey = GenerateCacheKey(request);
        string cacheKey = CacheKeys.ListApprovalRequests(generatedKey);

        // Retrieve the current list of cache keys and add the new key
        var cacheKeys = _cache.GetOrAdd(CacheKeys.TaskApprovalRequest(), () => new List<string>(), new TimeSpan(0, 45, 0));
        if (!cacheKeys.Contains(cacheKey))
        {
            cacheKeys.Add(cacheKey);
            _cache.Add(CacheKeys.TaskApprovalRequest(), cacheKeys);
        }

        // Try to get the cached result
        var cachedResult = await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            if (!string.IsNullOrEmpty(request.SearchQuery))
                request.SearchQuery = request.SearchQuery.ToLower().Trim();

            var approvalsQuery = _context.ProjectTaskApprovals
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchQuery))
            {
                string searchLike = $"%{request.SearchQuery}%";
                approvalsQuery = approvalsQuery.Where(pta =>
                    EF.Functions.Like(pta.Project!.Title, searchLike) ||
                    EF.Functions.Like(pta.Requester!.FirstName, searchLike) ||
                    EF.Functions.Like(pta.Requester.LastName, searchLike) ||
                    EF.Functions.Like(pta.Project!.CreatedBy!.FirstName, searchLike) ||
                    EF.Functions.Like(pta.Project.CreatedBy.LastName, searchLike));
            }

            return await approvalsQuery
                .OrderByDescending(pta => pta.CreatedAt)
                .ProjectToType<ProjectTaskApprovalView>()
                .ToPaginatedListAsync(request.PageIndex, request.PageSize);
        }, new TimeSpan(0, 45, 0));

        return new SuccessResult(cachedResult);
    }
}