using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace HCP.Service.Services;

public class FileService(BlobServiceClient blobServiceClient, IConfiguration configuration) : IFileService

{
    private readonly BlobServiceClient _blobServiceClient = blobServiceClient;
    private readonly string _containerName = configuration["AzureStorage:ContainerName"]!;

    public async Task<string> Upload(IFormFile file)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);

        string newFileName = $"{Guid.NewGuid().ToString()}_{file.FileName}";

        var blobClient = containerClient.GetBlobClient(newFileName);

        var blobHttpHeaders = new BlobHttpHeaders
        {
            ContentType = GetContentType(file.FileName),
        };

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(
                stream,
                new BlobUploadOptions { HttpHeaders = blobHttpHeaders }
            );
        }
        return blobClient.Uri.ToString();
    }
    public async Task<List<string>> UploadMultiple(List<IFormFile> files)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
        var uploadedUrls = new List<string>();

        foreach (var file in files)
        {
            string newFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var blobClient = containerClient.GetBlobClient(newFileName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = GetContentType(file.FileName),
            };

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });
            }

            uploadedUrls.Add(blobClient.Uri.ToString());
        }

        return uploadedUrls;
    }

    private string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" =>
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            _ => "application/octet-stream",
        };
    }
}