using HCP.DTOs.DTOs.CleaningServiceDTO;
using HCP.DTOs.DTOs.RequestDTO;
using System.Security.Claims;

namespace HCP.Service.Services.RequestService
{
    public interface IHandleRequestService
    {
        Task<List<PendingRequestDTO>> GetPendingCreateServiceRequestsAsync();
        Task<PendingRequestListDTO> GetPendingCreateServiceRequestsAsync(int? pageIndex, int? pageSize);
        Task<PendingRequestDTO> GetPendingCreateServiceDetailAsync(Guid id);
        Task<(bool Success, string Message)> UpdateServiceStatusAsync(ServiceStatusUpdateDto dto, ClaimsPrincipal userClaims);
        Task<List<ApprovalServiceDTO>> GetApprovedServiceByStaffIdAsync(ClaimsPrincipal user);
        Task<ApprovalServiceListDTO> GetAllApprovedServiceByStaffAsync(ClaimsPrincipal user, int? pageIndex, int? pageSize, string? status, string? searchByName);
        Task<RegistrationRequestListDTO> GetPendingHousekeeperRegistrationRequestsAsync(int? pageIndex, int? pageSize);
        Task<RegistrationRequestDetailDTO> GetPendingHousekeeperRegistrationRequestDetailAsync(ClaimsPrincipal staff, string housekeeperId);
        Task<(bool Success, string Message)> UpdateHouskeeperVerificationStatusAsync(RegistrationRequestStatusUpdateDto dto, ClaimsPrincipal userClaims);
        Task<RegistrationRequestListDTO> GetStaffRegistrationApproval(ClaimsPrincipal currentStaff, int? pageIndex, int? pageSize, string? status);
    }
}