using HCP.Service.DTOs.AdminManagementDTO;
using Microsoft.AspNetCore.Mvc;

namespace HCP.Service.Services.AdminManService
{
    public interface IAdminManService
    {
        Task<List<UserAdminDTO>> GetUsersByAdminCustom([FromQuery] bool includeStaff, [FromQuery] bool includeCustomers, [FromQuery] bool includeHousekeepers);
    }
}