// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View;

namespace Saharaviewpoint.Core.Interfaces;

public interface IFileService
{
    /// <summary>
    /// Uploads a file to Azure Blob storage and does not save the info to the database. The response
    /// contains the document object that can be saved to the database by the caller.
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="subFolder"></param>
    /// <param name="file">The file to be uploaded</param>
    /// <returns></returns>
    Task<Result<Document>> UploadFileInternal(string folder, string subFolder, IFormFile file);

    /// <summary>
    /// Uploads a file to Azure Blob Storage and stores the document info in the database.
    /// </summary>
    /// <param name="folder">The folder name (typically the user's UID)</param>
    /// <param name="subFolder">The sub folder (typically the project title or task name)</param>
    /// <param name="file">The file to be uploaded</param>
    /// <returns></returns>
    Task<Result<DocumentView>> UploadFile(string folder, string subFolder, IFormFile file);

    /// <summary>
    /// Gets a file from azure blob storage
    /// </summary>
    /// <param name="folder">The folder name (typically the user's UID)</param>
    /// <param name="subFolder">The sub folder (typically the project title or task name)</param>
    /// <param name="fileName">The name of the file to be retrieved.</param>
    /// <returns></returns>
    Task<FileStreamResult?> GetFileByPath(string folder, string subFolder, string fileName);

    Task<FileStreamResult?> GetGenericThumbnail(string filename);

    FileStreamResult? GetSvpLogo();

    /// <summary>
    /// Deletes a file from azure blob storage
    /// </summary>
    /// <param name="folder">The folder name (typically the user's UID)</param>
    /// <param name="subFolder">The sub folder (typically the project title or task name)</param>
    /// <param name="fileName">The name of the file to be retrieved.</param>
    /// <returns></returns>
    Task<Result> DeleteFile(string folder, string subFolder, string fileName);

    Task<Result<Document>> UploadTaskAttachment(string folder, string subFolder, IFormFile file,
        IProgress<int> progress);
}