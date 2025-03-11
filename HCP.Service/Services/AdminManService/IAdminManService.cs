using HCP.Service.DTOs.AdminManagementDTO;
using Microsoft.AspNetCore.Mvc;

namespace HCP.Service.Services.AdminManService
{
    public interface IAdminManService
    {
        Task<UserAdminListDTO> GetUsersByAdminCustom([FromQuery] bool includeStaff, [FromQuery] bool includeCustomers, [FromQuery] bool includeHousekeepers, int? pageIndex, int? pageSize);
    }
}