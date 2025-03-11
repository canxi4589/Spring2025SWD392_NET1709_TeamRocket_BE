using HCP.Repository.Entities;
using HCP.Service.DTOs.AdminManagementDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HCP.Service.Services.AdminManService
{
    public class AdminManService : IAdminManService
    {
        private readonly UserManager<AppUser> _userManager;

        public AdminManService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        private async Task<List<UserAdminDTO>> GetUsersByRoleAsync(string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            return users.Select(user => new UserAdminDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Status = "IsActive",
                Birthday = user.Birthday,
                Avatar = user.Avatar
            }).ToList();
        }
        public async Task<List<UserAdminDTO>> GetUsersByAdminCustom([FromQuery] bool includeStaff, [FromQuery] bool includeCustomers, [FromQuery] bool includeHousekeepers)
        {
            var users = new List<UserAdminDTO>();
            if (includeStaff)
            {
                users.AddRange(await GetUsersByRoleAsync("Staff"));
            }
            if (includeCustomers)
            {
                users.AddRange(await GetUsersByRoleAsync("Customer"));
            }
            if (includeHousekeepers)
            {
                users.AddRange(await GetUsersByRoleAsync("Housekeeper"));
            }
            return users;
        }
    }
}
