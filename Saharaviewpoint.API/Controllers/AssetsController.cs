// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.API.Controllers;

[Route("api/v1/assets")]
[ApiController]
[AllowAnonymous] // TODO: remove this
public class AssetsController(IFileService fileService) : BaseController
{
    private readonly IFileService _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));

    [HttpPost("{folder}/{subFolder}")]
    public async Task<IActionResult> UploadAsset(string folder, string subFolder, IFormFile file)
    {
        var result = await _fileService.UploadFile(folder, subFolder, file);
        return result.Success
            ? ProcessResponse(new SuccessResult(result.Status, result.Content))
            : ProcessResponse(new ErrorResult(result.Status, result.Title, result.Message));
    }

    [HttpGet("{folder}/{subFolder}/_thumbnail/{fileName}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetThumbnail(string folder, string subFolder, string fileName)
    {
        var result = await _fileService.GetFileByPath(folder, $"{subFolder}/_thumbnail", fileName);
        if (result != null)
            return result;

        return NotFound(new ErrorResult(StatusCodes.Status404NotFound, "File not found."));
    }

    [HttpGet("thumbnail/{fileName}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetThumbnail(string fileName)
    {
        var result = await _fileService.GetGenericThumbnail(fileName);
        if (result != null)
            return result;

        return NotFound(new ErrorResult(StatusCodes.Status404NotFound, "File not found."));
    }

    [HttpGet("{folder}/{subFolder}/{fileName}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAsset(string folder, string subFolder, string fileName)
    {
        var result = await _fileService.GetFileByPath(folder, subFolder, fileName);
        if (result != null)
            return result;

        return NotFound(new ErrorResult(StatusCodes.Status404NotFound, "File not found."));
    }

    [HttpGet("svp-logo")]
    [AllowAnonymous]
    public IActionResult GetSvpLogo()
    {
        var result = _fileService.GetSvpLogo();
        if (result != null)
            return result;

        return NotFound(new ErrorResult(StatusCodes.Status404NotFound, "File not found."));
    }
}