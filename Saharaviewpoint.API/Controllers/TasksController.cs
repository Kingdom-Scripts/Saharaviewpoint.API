using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.Input.Project;
using Saharaviewpoint.Core.Models.Input.Task;
using Saharaviewpoint.Core.Models.Utilities;
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
}