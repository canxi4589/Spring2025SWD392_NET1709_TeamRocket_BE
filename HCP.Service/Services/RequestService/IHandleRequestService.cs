using HCP.Service.DTOs.CleaningServiceDTO;

namespace HCP.Service.Services.RequestService
{
    public interface IHandleRequestService
    {
        Task<(bool Success, string Message)> UpdateServiceStatusAsync(ServiceStatusUpdateDto dto);
    }
}