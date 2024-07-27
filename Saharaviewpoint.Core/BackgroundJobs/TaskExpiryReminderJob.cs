// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.App.Constants;
using Saharaviewpoint.Models.Configurations;
using Saharaviewpoint.Models.Email;

namespace Saharaviewpoint.Core.BackgroundJobs;
internal class TaskExpiryReminderJob : IJob
{
    private readonly SaharaviewpointContext _context;
    private readonly IEmailService _emailService;
    private readonly BaseURLs _baseUrls;

    public TaskExpiryReminderJob(SaharaviewpointContext context, IEmailService emailService, IOptions<AppConfig> options)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        ArgumentNullException.ThrowIfNull(options);
        _baseUrls = options.Value.BaseURLs;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        DateTime currentDate = DateTime.Now;
        DateTime sevenDaysFromNow = currentDate.Date.AddDays(7);
        DateTime threeDaysFromNow = currentDate.Date.AddDays(3);
        DateTime lessThan24HoursFromNow = currentDate.AddHours(24);

        var pendingTasks = _context.Tasks
            .Where(x => x.Status == TaskStatusEnum.TODO || x.Status == TaskStatusEnum.IN_PROGRESS)
            .Where(task => task.DueDate.Date == sevenDaysFromNow.Date
                || task.DueDate.Date == threeDaysFromNow.Date
                || (task.DueDate > currentDate && task.DueDate <= lessThan24HoursFromNow))
            .ToList();

        foreach (var task in pendingTasks)
        {
            // get days left
            int daysLeft = (task.DueDate - currentDate).Days;
            int hoursLeft = (task.DueDate - currentDate).Hours;

            // get if it's hours left
            string actualLeft = daysLeft == 0
                ? hoursLeft < 2
                    ? $"{hoursLeft}hr"
                    : $"{hoursLeft}hrs"
                : daysLeft < 2
                    ? $"{daysLeft} day"
                    : $"{daysLeft} days";

            var projectData = await _context.Tasks
                .Where(t => t.Id == task.Id)
                .Select(t => new
                {
                    t.Project!.Title,
                    AssigneeEmail = t.Project.Assignee!.Email,
                    AssigneeFirstName = t.Project.Assignee.FirstName
                }).FirstAsync();

            var emailRequest = new GenericEmailModel
            {
                To = projectData.AssigneeEmail,
                Subject = "Task Expiry Reminder",
                Salutation = $"Hello {projectData.AssigneeFirstName},",
                PrimaryMessage = $"This is a reminder that the task below is due in {actualLeft}. " +
                    $"If the task has been completed, kindly match it as such.<br><br>" +
                    "<strong><span style=\"font-size:larger;\">Task Details</span></strong><br>" +
                    $"<strong>Project:</strong> {projectData.Title}<br>" +
                    $"<strong>Task Summary:</strong> {task.Summary}<br>" +
                    $"<strong>Current Status:</strong> {task.Status}<br>" +
                    $"<strong>Due Date:</strong> {task.DueDate}<br>",
                ClosingRemark = "Regards",
                ActionButton = new()
                {
                    Text = "Review Task",
                    Url = $"{_baseUrls.Admin}/tasks/all?projectId={task.ProjectId}&taskId={task.Id}"
                }
            };

            await _emailService.SendEmail(emailRequest);
        }
    }
}