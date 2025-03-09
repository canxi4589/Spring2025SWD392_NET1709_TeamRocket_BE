using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.DTOs.RequestDTO;
using System.Security.Claims;

namespace HCP.Service.Services.RequestService
{
    public interface IHandleRequestService
    {
        Task<List<PendingRequestDTO>> GetPendingCreateServiceRequestsAsync();
        Task<PendingRequestDTO> GetPendingCreateServiceDetailAsync(Guid id);
        Task<(bool Success, string Message)> UpdateServiceStatusAsync(ServiceStatusUpdateDto dto, ClaimsPrincipal userClaims);
        Task<List<ApprovalServiceDTO>> GetApprovedServiceByStaffIdAsync(ClaimsPrincipal user);
    }
}