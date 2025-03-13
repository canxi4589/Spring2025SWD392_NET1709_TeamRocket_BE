using HCP.Service.DTOs.AdminManagementDTO;
using static HCP.Service.DTOs.AdminManagementDTO.ServiceAdminDTO;

namespace HCP.Service.Services.AdminManService
{
    public interface IAdminServiceService
    {
        Task<ServiceAdminShowListDTO> GetAllServicesAsync(string? search, int? pageIndex, int? pageSize, int? day, int? month, int? year);
    }
}