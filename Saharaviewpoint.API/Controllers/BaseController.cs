// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.Models.Utilities;
using System.Net;

namespace Saharaviewpoint.API.Controllers;

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
            if (result.Status == StatusCodes.Status201Created)
            {
                return StatusCode(StatusCodes.Status201Created, result);
            }

            return Ok(result);
        }
        else if (result.Status == StatusCodes.Status401Unauthorized)
        {
            return Unauthorized(result);
        }
        else if (result.Status == StatusCodes.Status403Forbidden)
        {
            return StatusCode(StatusCodes.Status403Forbidden, result);
        }
        else if (result.Status == StatusCodes.Status404NotFound)
        {
            return NotFound(result);
        }
        else if (result.Status == StatusCodes.Status500InternalServerError)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result);
        }
        else
        {
            return BadRequest(result);
        }
    }
}