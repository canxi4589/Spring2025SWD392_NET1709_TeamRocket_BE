using Microsoft.AspNetCore.Http;

namespace HCP.Service.Integrations.BlobStorage
{
    public interface IBlobStorageService
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<List<string>> UploadFilesAsync(List<IFormFile> files);
    }
}