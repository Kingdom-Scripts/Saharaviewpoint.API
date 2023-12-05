using System.Net;
using Microsoft.AspNetCore.Mvc;
using Shareviewpoint.Core.Models.Utilities;

namespace Shareviewpoint.API.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    /// <summary>
    /// Returns the appropriate HTTP Response.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns></returns>
    protected IActionResult ProcessResponse(Result result)
    {
        if (result.Success)
        {
            return Ok(result);
        }
        else if (result.Status == StatusCodes.Status401Unauthorized)
        {
            return Unauthorized(result);
        }
        else if (result.Status == StatusCodes.Status403Forbidden)
        {
            return StatusCode((int)HttpStatusCode.Forbidden, result);
        }
        else if (result.Status == StatusCodes.Status404NotFound)
        {
            return NotFound(result);
        }
        else if (result.Status == StatusCodes.Status500InternalServerError)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, result);
        }
        else
        {
            return StatusCode((int)HttpStatusCode.BadRequest, result);
        }
    }
}