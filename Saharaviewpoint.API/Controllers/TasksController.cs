// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Models.Input;
using Saharaviewpoint.Models.Input.Project;
using Saharaviewpoint.Models.Input.Task;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View;
using Saharaviewpoint.Models.View.Task;

namespace Saharaviewpoint.API.Controllers;

[ApiController]
[Route("api/v1/tasks")]
public class TasksController(ITaskService taskService) : BaseController
{
    private readonly ITaskService _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResult<TaskDetailView>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> CreateTask([FromForm] TaskModel model)
    {
        var result = await _taskService.CreateTask(model);
        return ProcessResponse(result);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<List<TaskView>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> ListTasks([FromQuery] TaskSearchModel request)
    {
        var result = await _taskService.ListTasks(request);
        return ProcessResponse(result);
    }

    [HttpGet("{taskId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<TaskDetailView>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    public async Task<IActionResult> GetTask(int taskId)
    {
        var result = await _taskService.GetTask(taskId);
        return ProcessResponse(result);
    }

    [HttpDelete("{taskId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    public async Task<IActionResult> DeleteTask(int taskId)
    {
        var result = await _taskService.DeleteTask(taskId);
        return ProcessResponse(result);
    }

    [HttpGet("{taskId}/attachments")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<List<DocumentView>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    public async Task<IActionResult> ListAttachments(int taskId)
    {
        var result = await _taskService.ListAttachments(taskId);
        return ProcessResponse(result);
    }

    // TODO: fix this for dynamic file upload
    [HttpPost("{taskId}/attachments")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResult<DocumentView>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> UploadAttachment(int taskId, [FromForm] FileUploadModel file,
        [FromServices] IHttpContextAccessor accessor)
    {
        // var response = accessor.HttpContext.Response;
        // response.ContentType = "application/json";
        // response.Headers.Add("Cache-Control", "no-cache");
        // response.Headers.Add("Connection", "keep-alive");

        Response.StatusCode = 200;
        Response.ContentType = "text/event-stream";
        Response.ContentLength = 10000;

        var sw = new StreamWriter(Response.Body);

        var progress = new Progress<int>(async percentage =>
        {
            if (percentage > 96)
            {
                int wait = 2;
            }

            // Send progress update to client
            var progressResponse = new SuccessResult(new { Percentage = percentage });
            string json = JsonConvert.SerializeObject(progressResponse);

            // Write progress update to response stream and flush immediately
            // var data = Encoding.UTF8.GetBytes(json);
            // response.Body.WriteAsync(data, 0, data.Length);
            // response.Body.FlushAsync();
            await sw.WriteAsync(percentage.ToString());
            await sw.FlushAsync();
        });

        var result = await _taskService.AddAttachmentToTask(taskId, file, progress);
        return ProcessResponse(result);
    }

    [HttpDelete("{taskId}/attachments/{documentId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    public async Task<IActionResult> RemoveAttachment(int taskId, int documentId)
    {
        var result = await _taskService.RemoveAttachmentFromTask(taskId, documentId);
        return ProcessResponse(result);
    }

    [HttpGet("{taskId}/logs")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<List<TaskLogView>>))]
    public async Task<IActionResult> ListLogs(int taskId, [FromQuery] PagingOptionModel request)
    {
        var result = await _taskService.ListLogs(taskId, request);
        return ProcessResponse(result);
    }

    [HttpGet("{projectId}/board")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<List<BoardTaskView>>))]
    public async Task<IActionResult> ListBoardTasks(int projectId)
    {
        var result = await _taskService.ListBoardTasks(projectId);
        return ProcessResponse(result);
    }

    [HttpPatch("{taskId}/status")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    public async Task<IActionResult> ChangeTaskStatus(int taskId, [FromBody] TaskStatusModel model)
    {
        var result = await _taskService.ChangeTaskStatus(taskId, model);
        return ProcessResponse(result);
    }

    [HttpPatch("{taskId}/due-date")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<TaskView>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    public async Task<IActionResult> ChangeDueDate(int taskId, [FromBody] TaskDueDateModel model)
    {
        var result = await _taskService.ChangeDueDate(taskId, model);
        return ProcessResponse(result);
    }

    [HttpPost("{taskId}/comments")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResult<TaskCommentView>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> AddComment(int taskId, [FromBody] CommentModel model)
    {
        var result = await _taskService.AddComment(taskId, model);
        return ProcessResponse(result);
    }

    [HttpDelete("{taskId}/comments/{commentId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    public async Task<IActionResult> RemoveComment(int taskId, int commentId)
    {
        var result = await _taskService.RemoveComment(taskId, commentId);
        return ProcessResponse(result);
    }

    [HttpGet("{taskId}/comments")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<List<TaskCommentView>>))]
    public async Task<IActionResult> ListComments(int taskId, [FromQuery] PagingOptionModel request)
    {
        var result = await _taskService.ListComments(taskId, request);
        return ProcessResponse(result);
    }
}