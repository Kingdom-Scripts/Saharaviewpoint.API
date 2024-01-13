using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.Core.Models.Utilities;
using Saharaviewpoint.Core.Models.View;

namespace Saharaviewpoint.Core.Interfaces;

public interface IFileService
{
    Task<Result<DocumentView>> UploadFile(string projectName, IFormFile file);
    Task<FileStreamResult> GetFileByPath(string folder, string fileName);
}
