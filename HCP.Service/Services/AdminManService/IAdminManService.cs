using HCP.Service.DTOs.AdminManagementDTO;
using Microsoft.AspNetCore.Mvc;

namespace HCP.Service.Services.AdminManService
{
    public interface IAdminManService
    {
        Task<UserAdminListDTO> GetUsersByAdminCustom(bool includeStaff, bool includeCustomers, bool includeHousekeepers, int? pageIndex, int? pageSize);
    }
}