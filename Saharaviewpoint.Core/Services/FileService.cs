// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Azure.Storage.Blobs;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.App.Constants;
using Saharaviewpoint.Models.Configurations;
using Saharaviewpoint.Models.Input.Auth;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View;
using Serilog;

namespace Saharaviewpoint.Core.Services;

// TODO: implement cache
public class FileService : IFileService
{
    private readonly SaharaviewpointContext _context;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly UserSession _userSession;
    private readonly IWebHostEnvironment _hostEnvironment;

    // TODO: add caching
    public FileService(IOptions<AppConfig> appConfig, IOptions<KeyVaultConfig> keyVaultConfig, UserSession userSession,
        SaharaviewpointContext context, IWebHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        ArgumentNullException.ThrowIfNull(keyVaultConfig);

        // TODO: remove this api call from here
        //var keyVault = keyVaultConfig.Value;
        //var credential = new ClientSecretCredential(keyVault.DirectoryID, keyVault.ClientId, keyVault.ClientSecret);

        // var client = new SecretClient(new Uri(keyVault.KeyVaultURL), credential);

        // string connectionString = $"{client.GetSecret("StorageKey--Saharaviewpoint").Value.Value}";

        string connectionString =
            "DefaultEndpointsProtocol=https;AccountName=svpstoragedev;AccountKey=HGWzG6y5UG06ZnrauWD+fqkAVMcdyea/v4V8Q4qaA7MUvdjSvZe//K+Db4x7PWWw9JtyXkscC7Yj+AStRJ9Wug==;EndpointSuffix=core.windows.net";
        _blobServiceClient = new BlobServiceClient(connectionString);
        _userSession = userSession;
        _context = context;

        // set up tinify
        TinifyAPI.Tinify.Key = appConfig.Value.TinifyKey;
    }

    public async Task<Result<Document>> UploadFileInternal(string folder, string subFolder, IFormFile file)
    {
        return await Upload(folder, subFolder, file);
    }

    public async Task<Result<DocumentView>> UploadFile(string folder, string subFolder, IFormFile file)
    {
        var result = await Upload(folder, subFolder, file);
        if (!result.Success)
            return new ErrorResult<DocumentView>(result.Title, result.Message);

        await _context.Documents.AddAsync(result.Content);

        int saved = await _context.SaveChangesAsync();

        return saved > 0
            ? new SuccessResult<DocumentView>(result.Content.Adapt<DocumentView>())
            : new ErrorResult<DocumentView>("Saving file failed");
    }

    public async Task<FileStreamResult> GetFileByPath(string folder, string subFolder, string fileName)
    {
        return await GetFile(folder, subFolder, fileName);
    }

    public async Task<FileStreamResult> GetGenericThumbnail(string fileName)
    {
        var blobContainer = _blobServiceClient.GetBlobContainerClient("thumbnails");

        var blobClient = blobContainer.GetBlobClient(fileName);

        if (!await blobClient.ExistsAsync())
        {
            return null;
        }

        var stream = await blobClient.OpenReadAsync();
        string? contentType = blobClient.GetProperties().Value.ContentType;
        return new FileStreamResult(stream, contentType)
        {
            FileDownloadName = fileName
        };
    }

    public async Task<Result> DeleteFile(string folder, string subFolder, string fileName)
    {
        var blobContainer = _blobServiceClient.GetBlobContainerClient(folder);

        var blobClient = blobContainer.GetBlobClient($"{subFolder}/{fileName}");

        if (!await blobClient.ExistsAsync())
        {
            return new ErrorResult("File not found");
        }

        bool deleted = await blobClient.DeleteIfExistsAsync();

        return deleted ? new SuccessResult() : new ErrorResult("Failed to delete file");
    }

    private async Task<Result<Document>> Upload(string folder, string subFolder, IFormFile file)
    {
        try
        {
            string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            string fileType = GetDocumentType(ext);

            var containerClient = _blobServiceClient.GetBlobContainerClient(folder);

            await containerClient.CreateIfNotExistsAsync();

            string fileUploadName = $"{Guid.NewGuid()}{ext}";
            if (fileType != DocumentTypes.IMAGE)
            {
                var blobClient = containerClient.GetBlobClient($"{subFolder}/{fileUploadName}");

                // Upload data
                await blobClient.UploadAsync(file.OpenReadStream(), true);
            }
            else
            {
                await SaveImageAsync(containerClient, subFolder, fileUploadName, file);
            }

            var document = new Document
            {
                Name = file.FileName,
                Type = fileType,
                Url = $"{folder}/{subFolder}/{fileUploadName}",
                ThumbnailUrl = fileType == DocumentTypes.IMAGE
                    ? $"{folder}/{subFolder}/_thumbnail/{fileUploadName}"
                    : $"{folder}/{subFolder}/_thumbnail/{fileType}.png",
                CreatedById = _userSession.UserId
            };

            return new SuccessResult<Document>(document);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error uploading file");
            return new ErrorResult<Document>("An unexpected error occurred while uploading your file(s)");
        }
    }

    public FileStreamResult GetSvpLogo()
    {
        // get logo from file storage
        string filePath = Path.Combine(_hostEnvironment.WebRootPath, "images", "svp-logo.svg");

        if (!File.Exists(filePath))
        {
            return null;
        }

        var stream = new FileStream(filePath, FileMode.Open);

        return new FileStreamResult(stream, "image/svg+xml")
        {
            FileDownloadName = "svp-logo.svg"
        };
    }

    private async Task<FileStreamResult> GetFile(string folder, string subFolder, string fileName)
    {
        var blobContainer = _blobServiceClient.GetBlobContainerClient(folder);

        var blobClient = blobContainer.GetBlobClient($"{subFolder}/{fileName}");

        if (!await blobClient.ExistsAsync())
        {
            return null;
        }

        var stream = await blobClient.OpenReadAsync();
        string contentType = blobClient.GetProperties().Value.ContentType;
        return new FileStreamResult(stream, contentType)
        {
            FileDownloadName = fileName
        };
    }

    private static async Task<BlobClient> SaveImageAsync(BlobContainerClient containerClient, string subFolder,
        string fileUploadName, IFormFile image)
    {
        using (var stream = new MemoryStream())
        {
            await image.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            // Compress the image using Tinify
            var source = await TinifyAPI.Tinify.FromBuffer(stream.ToArray());

            // get thumbnail
            byte[]? thumbnailFile = await source
                .Preserve("copyright", "creation")
                .Resize(new
                {
                    method = "thumb",
                    width = 150,
                    height = 150
                }).ToBuffer();

            // compress original
            byte[] optimizedFile = await source
                .Preserve("copyright", "creation")
                .ToBuffer();

            var thumbnailClient = containerClient.GetBlobClient($"{subFolder}/_thumbnail/{fileUploadName}");
            var imageClient = containerClient.GetBlobClient($"{subFolder}/{fileUploadName}");

            // Upload data
            await thumbnailClient.UploadAsync(new MemoryStream(thumbnailFile), true);
            await imageClient.UploadAsync(new MemoryStream(optimizedFile), true);

            return imageClient;
        }
    }

    private static string GetDocumentType(string extension)
    {
        if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
        {
            return DocumentTypes.IMAGE;
        }
        else if (extension == ".pdf")
        {
            return DocumentTypes.PDF;
        }
        else if (extension == ".doc" || extension == ".docx")
        {
            return DocumentTypes.WORD_DOCUMENT;
        }
        // Excel file or CSV
        else if (extension == ".xls" || extension == ".xlsx" || extension == ".csv")
        {
            return DocumentTypes.EXCEL_DOCUMENT;
        }
        else
        {
            return DocumentTypes.UNKNWON;
        }
    }
}