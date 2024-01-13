using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

    public FileService(IOptions<AppConfig> appConfig, IOptions<KeyVaultConfig> keyVaultConfig, UserSession userSession, SaharaviewpointContext context)
    {
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

    public async Task<Result<DocumentView>> UploadFile(string projectName, IFormFile file)
    {
        projectName = projectName.ToLower().Trim();
        string fileName = file.FileName.Split(".")[0];
        string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        string fileType = GetDocumentType(ext);

        var containerClient = _blobServiceClient.GetBlobContainerClient(projectName);

        await containerClient.CreateIfNotExistsAsync();

        if (fileType != DocumentTypes.IMAGE)
        {
            var blobClient = containerClient.GetBlobClient(file.FileName);

            // Upload data
            await blobClient.UploadAsync(file.OpenReadStream(), true);
        } else
        {
            await SaveImageAsync(containerClient, file);
        }        

        var document = new Document
        {
            Folder = projectName,
            Name = fileName,
            Extension = ext,
            Type = fileType,
            CreatedById = _userSession.UserId
        };

        await _context.AddAsync(document);

        int saved = await _context.SaveChangesAsync();

        if (saved > 0)
            return new SuccessResult<DocumentView>(document.Adapt<DocumentView>());

        return new ErrorResult<DocumentView>("Saving file failed");
    }

    public async Task<FileStreamResult> GetFileByPath(string folder, string fileName)
    {
        return await GetFile(folder.ToLower().Trim(), fileName);
    }

    //public async Task<FileStreamResult> GetDocumentById(int id)
    //{
    //    var document = await _context.Documents.FindAsync(id);
    //    if (document == null) return null;

    //    return await GetFile(document.Folder, $"{document.Name}.{document.Extension}");
    //}

    private async Task<FileStreamResult> GetFile(string folder, string fileName)
    {
        var blobContainer = _blobServiceClient.GetBlobContainerClient(folder);

        var blobClient = blobContainer.GetBlobClient(fileName);

        if (!await blobClient.ExistsAsync())
        {
            return null;
        }

        var stream = await blobClient.OpenReadAsync();
        var contentType = blobClient.GetProperties().Value.ContentType;
        return new FileStreamResult(stream, contentType)
        {
            FileDownloadName = fileName
        };
    }

    private static async Task<BlobClient> SaveImageAsync(BlobContainerClient containerClient, IFormFile image)
    {
        using (var stream = new MemoryStream())
        {
            await image.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            // Compress the image using Tinify
            var source = await TinifyAPI.Tinify.FromBuffer(stream.ToArray());

            // get thumbnail
            var thumbnailFile = await source
                .Preserve("copyright", "creation")
                .Resize(new
                    {
                        method = "thumb",
                        width = 150,
                    }).ToBuffer();

            // compress original
            byte[] optimizedFile = await source
                .Preserve("copyright", "creation")
                .ToBuffer();

            var thumbnailClient = containerClient.GetBlobClient($"thumbnails/{image.FileName}");
            var imageClient = containerClient.GetBlobClient(image.FileName);

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