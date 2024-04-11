using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.Input;
using Saharaviewpoint.Core.Models.Input.Project;
using Saharaviewpoint.Core.Models.Input.Task;
using Saharaviewpoint.Core.Models.Utilities;
using Saharaviewpoint.Core.Models.View;
using Saharaviewpoint.Core.Models.View.Task;

namespace Saharaviewpoint.API.Controllers;

[ApiController]
[Route("api/v1/tasks")]
public class TasksController : BaseController
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService) =>
        _taskService = taskService ?? throw new ArgumentNullException(nameof(taskService));

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

    [HttpGet("{taskId}/attachments")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<List<DocumentView>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    public async Task<IActionResult> ListAttachments(int taskId)
    {
        var result = await _taskService.ListAttachments(taskId);
        return ProcessResponse(result);
    }

    [HttpPost("{taskId}/attachments")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResult<DocumentView>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> UploadAttachment(int taskId, [FromForm] FileUploadModel file, [FromServices] IHttpContextAccessor accessor)
    {
        var response = accessor.HttpContext.Response;

        var progress = new Progress<int>(percentage =>
        {
            // Send progress update to client
            var progressResponse = new SuccessResult(new { Percentage = percentage });
            string json = JsonConvert.SerializeObject(progressResponse);
            response.ContentType = "application/json";
            response.WriteAsync(json);
            response.Body.FlushAsync(); // Ensure data is sent immediately
        });

        var result = await _taskService.AddAttachmentToTask(taskId, file, progress);
        return ProcessResponse(result);
    }
}