
using Microsoft.AspNetCore.Http;

 namespace HCP.Service.Services;


public interface IFileService
{
    Task<string> Upload(IFormFile file);
}