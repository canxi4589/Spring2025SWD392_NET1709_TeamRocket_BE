using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.DTOs.RequestDTO;

namespace HCP.Service.Services.RequestService
{
    public interface IHandleRequestService
    {
        Task<List<PendingRequestDTO>> GetPendingCreateServiceRequestsAsync();
        Task<(bool Success, string Message)> UpdateServiceStatusAsync(ServiceStatusUpdateDto dto);
    }
}