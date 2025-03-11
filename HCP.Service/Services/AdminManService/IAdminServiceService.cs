using HCP.Service.DTOs.AdminManagementDTO;

namespace HCP.Service.Services.AdminManService
{
    public interface IAdminServiceService
    {
        Task<List<ServiceAdminDTO.ServiceAdminShowDTO>> GetAllServicesAsync();
    }
}