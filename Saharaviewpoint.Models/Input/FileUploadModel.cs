using FluentValidation;
using Microsoft.AspNetCore.Http;
using Saharaviewpoint.Core.Utilities;

namespace Saharaviewpoint.Models.Input
{
    public class FileUploadModel
    {
        public IFormFile File { get; set; }
    }

    public class FileUploadValidator : AbstractValidator<FileUploadModel>
    {
        public FileUploadValidator()
        {
            RuleFor(model => model.File)
                .Custom((file, context) =>
                {
                    var validationResult = CustomFileValidator.HaveValidFile(file);
                    if (!validationResult.IsValid)
                    {
                        context.AddFailure($"File: {validationResult.ErrorMessage}");
                    }
                });
        }
    }
}