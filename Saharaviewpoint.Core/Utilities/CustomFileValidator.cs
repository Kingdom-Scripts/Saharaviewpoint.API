using Microsoft.AspNetCore.Http;

namespace Saharaviewpoint.Core.Utilities;

public static class CustomFileValidator
{
    public class FileValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public static FileValidationResult HaveValidFile(IFormFile? design)
    {
        if (design == null)
        {
            return new FileValidationResult { IsValid = true };
        }

        if (design.Length == 0)
        {
            return new FileValidationResult { IsValid = false, ErrorMessage = "No file provided or the file is empty." };
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var maxFileSize = 5 * 1024 * 1024; // 5 MB

        var fileExtension = Path.GetExtension(design.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(fileExtension))
        {
            return new FileValidationResult { IsValid = false, ErrorMessage = "Invalid file extension." };
        }

        if (design.Length > maxFileSize)
        {
            return new FileValidationResult { IsValid = false, ErrorMessage = "File size exceeds the maximum allowed size." };
        }

        // Additional custom validation logic if needed

        return new FileValidationResult { IsValid = true, ErrorMessage = null };
    }
}