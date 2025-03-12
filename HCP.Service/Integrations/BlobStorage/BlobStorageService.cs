using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.Integrations.BlobStorage
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string _containerName;

        public BlobStorageService(IConfiguration configuration)
        {
            string connectionString = configuration["AzureStorage:BlobConnectionString"];
            _containerName = configuration["AzureStorage:ContainerName"];
            _blobServiceClient = new BlobServiceClient(connectionString);
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            string blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);

            // Generate a SAS URL for secure access
            BlobSasBuilder sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                BlobName = blobName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5) // Expires in 5 min lul
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri.ToString();
        }
        public async Task<List<string>> UploadFilesAsync(List<IFormFile> files)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            List<string> sasUrls = new List<string>();

            foreach (var file in files)
            {
                string blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                await using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, true);

                BlobSasBuilder sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = _containerName,
                    BlobName = blobName,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5) 
                };
                sasBuilder.SetPermissions(BlobSasPermissions.Read);

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
                sasUrls.Add(sasUri.ToString());
            }

            return sasUrls;
        }
        public async Task<List<string>> UploadFilesAsyncWithoutSAS(List<IFormFile> files)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            List<string> fileUrls = new List<string>();

            foreach (var file in files)
            {
                string blobName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = GetContentType(file.FileName),
                };

                await using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders });

                fileUrls.Add(blobClient.Uri.ToString());
            }

            return fileUrls;
        }
        private string GetContentType(string fileName)
        {
            var mimeTypes = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
            {
                [".jpg"] = "image/jpeg",
                [".jpeg"] = "image/jpeg",
                [".png"] = "image/png",
                [".gif"] = "image/gif",
                [".pdf"] = "application/pdf",
                [".doc"] = "application/msword",
                [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                [".xls"] = "application/vnd.ms-excel",
                [".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            };

            string extension = Path.GetExtension(fileName)?.ToLowerInvariant();
            return mimeTypes.TryGetValue(extension, out var contentType) ? contentType : "application/octet-stream";
        }

    }

}
