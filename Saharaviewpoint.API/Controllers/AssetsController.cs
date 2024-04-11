using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.API.Controllers;

[Route("api/v1/assets")]
[ApiController]
[AllowAnonymous] // TODO: remove this
public class AssetsController : BaseController
{
    private readonly IFileService _fileService;

    public AssetsController(IFileService fileService)
    {
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
    }

    [HttpPost("{folder}/{subFolder}")]
    public async Task<IActionResult> UploadAsset(string folder, string subFolder, IFormFile file)
    {
        var result = await _fileService.UploadFile(folder, subFolder, file);
        return result.Success
            ? ProcessResponse(new SuccessResult(result.Status, result.Content))
            : ProcessResponse(new ErrorResult(result.Status, result.Title, result.Message));
    }

    [HttpGet("{folder}/{subFolder}/_thumbnail/{fileName}")]
    [AllowAnonymous] // TODO: remove this
    public async Task<IActionResult> GetThumbnail(string folder, string subFolder, string fileName)
    {
        var result = await _fileService.GetFileByPath(folder, $"{subFolder}/_thumbnail", fileName);
        if (result != null)
            return result;

        return NotFound(new ErrorResult(StatusCodes.Status404NotFound, "File not found."));
    }

    [HttpGet("{folder}/{subFolder}/{fileName}")]
    [AllowAnonymous] // TODO: remove this
    public async Task<IActionResult> GetAsset(string folder, string subFolder, string fileName)
    {
        var result = await _fileService.GetFileByPath(folder, subFolder, fileName);
        if (result != null)
            return result;

        return NotFound(new ErrorResult(StatusCodes.Status404NotFound, "File not found."));
    }
}