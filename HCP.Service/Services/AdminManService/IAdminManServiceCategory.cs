using HCP.Service.DTOs.AdminManagementDTO;
using static HCP.Service.DTOs.AdminManagementDTO.ServiceCategoryAdminDTO;

namespace HCP.Service.Services.AdminManService
{
    public interface IAdminManServiceCategory
    {
        Task<ServiceCategoryAdminShowListDTO> GetAllServiceCategoriesAsync(int? pageIndex, int? pageSize);
    }
}