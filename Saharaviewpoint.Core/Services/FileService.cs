using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Models.App;
using Saharaviewpoint.Core.Models.App.Constants;
using Saharaviewpoint.Core.Models.Configurations;
using Saharaviewpoint.Core.Models.Input.Auth;
using Saharaviewpoint.Core.Models.Utilities;
using Saharaviewpoint.Core.Models.View;

namespace Saharaviewpoint.Core.Services;

public class FileService : IFileService
{
    private SaharaviewpointContext _context;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly UserSession _userSession;
    private readonly ILogger<FileService> _logger;

    public FileService(IOptions<AppConfig> appConfig, IOptions<KeyVaultConfig> keyVaultConfig, UserSession userSession,
        SaharaviewpointContext context, ILogger<FileService> logger)
    {
        _logger = logger;

        if (keyVaultConfig == null) throw new ArgumentNullException(nameof(keyVaultConfig));

        var keyVault = keyVaultConfig.Value;
        var credential = new ClientSecretCredential(keyVault.DirectoryID, keyVault.ClientId, keyVault.ClientSecret);

        var client = new SecretClient(new Uri(keyVault.KeyVaultURL), credential);

        string connectionString = $"{client.GetSecret("StorageKey--Saharaviewpoint").Value.Value}";

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

    public async Task<FileStreamResult?> GetFileByPath(string folder, string subFolder, string fileName)
    {
        return await GetFile(folder, subFolder, fileName);
    }

    private async Task<Result<Document>> Upload(string folder, string subFolder, IFormFile file)
    {
        try
        {
            string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            string fileType = GetDocumentType(ext);

            // string containerName = CreateContainerNameFromProjectTitle(subFolder);
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
            _logger.LogError(ex, "Error uploading file");
            return new ErrorResult<Document>("An unexpected error occurred while uploading your file(s)");
        }
    }

    private async Task<FileStreamResult?> GetFile(string folder, string subFolder, string fileName)
    {
        var blobContainer = _blobServiceClient.GetBlobContainerClient(folder);

        var blobClient = blobContainer.GetBlobClient($"{subFolder}/{fileName}");

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
        else
        {
            return DocumentTypes.UNKNWON;
        }
    }
}