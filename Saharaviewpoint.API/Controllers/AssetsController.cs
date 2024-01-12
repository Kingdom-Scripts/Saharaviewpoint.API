using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.API.Controllers;

[Route("api/v1/assets")]
[ApiController]
[AllowAnonymous] // TODO: remove this
// [Authorize] TODO: activate this
public class AssetsController : BaseController
{
    private readonly IFileService _fileService;

    public AssetsController(IFileService fileService)
    {
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
    }

    [HttpPost("{projectName}")]
    public async Task<IActionResult> UploadAsset(string projectName, IFormFile file)
    {
        var result = await _fileService.UploadFile(projectName, file);
        if (result.Success)
        {
            return ProcessResponse(new SuccessResult(result.Status, result.Content));
        }
        else
        {
            return ProcessResponse(new ErrorResult(result.Status, result.Title, result.Message));
        }
    }

    [HttpGet("{folder}/thumbnails/{fileName}")]
    public async Task<IActionResult> GetThumbnail(string folder, string fileName)
    {
        var result = await _fileService.GetFileByPath(folder, $"thumbnails/{fileName}");
        if (result != null)
            return result;

        return NotFound(new ErrorResult(StatusCodes.Status404NotFound, "File not found."));
    }

    [HttpGet("{folder}/{fileName}")]
    public async Task<IActionResult> GetAsset(string folder, string fileName)
    {
        var result = await _fileService.GetFileByPath(folder, fileName);
        if (result != null)
            return result;

        return NotFound(new ErrorResult(StatusCodes.Status404NotFound, "File not found."));
    }
}
