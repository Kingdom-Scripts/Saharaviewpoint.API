using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Saharaviewpoint.Models.Utilities;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace Saharaviewpoint.Core.Extensions;

public class CustomResultFactory : IFluentValidationAutoValidationResultFactory
{
    public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails validationProblemDetails)
    {
        var errorResponse = new ErrorResult("Validation Errors", "")
        {
            ValidationErrors = validationProblemDetails?.Errors
        };

        return new BadRequestObjectResult(errorResponse);
    }
}