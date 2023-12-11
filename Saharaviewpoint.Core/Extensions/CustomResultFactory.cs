using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;
using Saharaviewpoint.Core.Models.Utilities;

namespace Saharaviewpoint.Core.Extensions
{
    public class CustomResultFactory : IFluentValidationAutoValidationResultFactory
    {
        public IActionResult CreateActionResult(ActionExecutingContext context, ValidationProblemDetails validationProblemDetails)
        {
            var errorResponse = new ErrorResult("Validation Errors", "");
            errorResponse.ValidationErrors = validationProblemDetails?.Errors;

            return new BadRequestObjectResult(errorResponse);
        }
    }
}
