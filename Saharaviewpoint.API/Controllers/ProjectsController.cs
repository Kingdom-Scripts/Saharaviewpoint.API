using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.Input.Project;
using Saharaviewpoint.Core.Models.Utilities;
using Saharaviewpoint.Core.Models.View.Project;

namespace Saharaviewpoint.API.Controllers;

[ApiController]
[Route("api/v1/projects")]
public class ProjectsController : BaseController
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService ?? throw new ArgumentNullException(nameof(projectService));
    }

    [AllowAnonymous] // TODO: remove this
    [HttpGet]
    [Authorize(Policy = "BasicAccess")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<IEnumerable<ProjectDetailView>>))]
    public async Task<IActionResult> ListProjects([FromQuery] ProjectSearchModel paging)
    {
        var result = await _projectService.ListProjects(paging);
        return ProcessResponse(result);
    }

    [AllowAnonymous] // TODO: remove this
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<ProjectDetailView>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    public async Task<IActionResult> GetProject(int id)
    {
        var result = await _projectService.GetProject(id);
        return ProcessResponse(result);
    }

    [AllowAnonymous] // TODO: remove this
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SuccessResult<ProjectDetailView>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BadErrorResult))]
    public async Task<IActionResult> CreateProject([FromForm] ProjectModel model)
    {
        var result = await _projectService.CreateProject(model);
        return ProcessResponse(result);
    }

    [AllowAnonymous] // TODO: remove this
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<ProjectDetailView>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> UpdateProject(int id, [FromForm] ProjectModel model)
    {
        var result = await _projectService.UpdateProject(id, model);
        return ProcessResponse(result);
    }

    [AllowAnonymous] // TODO: remove this
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var result = await _projectService.DeleteProject(id);
        return ProcessResponse(result);
    }

    [AllowAnonymous] // TODO: remove this
    [HttpPost("{id:int}/reassign")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<ProjectDetailView>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> ReassignProject(int id, [FromBody] ReassignProjectModel model)
    {
        var result = await _projectService.ReassignProject(id, model);
        return ProcessResponse(result);
    }

    [AllowAnonymous] // TODO: remove this
    [HttpPut("{id:int}/update-status")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SuccessResult<ProjectDetailView>))]
    [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ErrorResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResult))]
    public async Task<IActionResult> UpdateProjectStatus(int id, [FromBody] ProjectStatusModel model)
    {
        var result = await _projectService.UpdateProjectStatus(id, model);
        return ProcessResponse(result);
    }

    [HttpPost("types")]
    [AllowAnonymous] // TODO: remove this
    public async Task<IActionResult> CreateType(TaskModel model)
    {
        var result = await _projectService.CreateType(model);
        return ProcessResponse(result);
    }

    [AllowAnonymous] // TODO: remove this
    [HttpDelete("types/{id:int}")]
    public async Task<IActionResult> DeleteType(int id)
    {
        var result = await _projectService.DeleteType(id);
        return ProcessResponse(result);
    }

    [AllowAnonymous] // TODO: remove this
    [HttpGet("types")]
    public async Task<IActionResult> ListTypes(string searchTerm)
    {
        var result = await _projectService.ListTypes(searchTerm);
        return ProcessResponse(result);
    }
}