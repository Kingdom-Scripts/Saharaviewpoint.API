using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Saharaviewpoint.Core.Models.Configurations;

namespace Saharaviewpoint.Core.Models.Input;

public class ImageUploadModel
{
    public IFormFile File { get; set; }
}

public class ImageUploadModelValidator : AbstractValidator<ImageUploadModel>
{
    private readonly FileSettings _fileSettings;

    public ImageUploadModelValidator(IOptions<AppConfig> appConfig)
    {
        _fileSettings = appConfig.Value.FileSettings;

        RuleFor(model => model.File)
            .Must(file => file.Length > 0)
            .WithMessage("File is unreadable")
            .Must(file => file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) && file.Length <= _fileSettings.MaxSizeLength)
            .WithMessage(file => $"File is too large. Max file size for images is {_fileSettings.MaxSizeLength / (1024 * 1024)}MB")
            .Must(file => !string.IsNullOrEmpty(Path.GetExtension(file.FileName).ToLowerInvariant()) &&
                           _fileSettings.PermittedFileTypes.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
            .WithMessage(file => $"File is invalid. Please upload only a {string.Join(", ", _fileSettings.PermittedFileTypes)} file");
    }
}
