// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Saharaviewpoint.Models.Configurations;

namespace Saharaviewpoint.Models.Input;

public class ImageUploadModel
{
    public IFormFile File { get; set; }
}

public class ImageUploadModelValidator : AbstractValidator<ImageUploadModel>
{
    public ImageUploadModelValidator(IOptions<AppConfig> appConfig)
    {
        var fileSettings = appConfig.Value.FileSettings;

        RuleFor(model => model.File)
            .Must(file => file.Length > 0)
            .WithMessage("File is unreadable")
            .Must(file => file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) && file.Length <= fileSettings.MaxSizeLength)
            .WithMessage(_ => $"File is too large. Max file size for images is {fileSettings.MaxSizeLength / (1024 * 1024)}MB")
            .Must(file => !string.IsNullOrEmpty(Path.GetExtension(file.FileName).ToLowerInvariant()) &&
                           fileSettings.PermittedFileTypes.Contains(Path.GetExtension(file.FileName).ToLowerInvariant()))
            .WithMessage(_ => $"File is invalid. Please upload only a {string.Join(", ", fileSettings.PermittedFileTypes)} file");
    }
}