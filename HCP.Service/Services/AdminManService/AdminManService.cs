using HCP.Repository.Entities;
using HCP.Service.DTOs.AdminManagementDTO;
using HCP.Service.Services.ListService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using static HCP.Service.DTOs.AdminManagementDTO.ServiceCategoryAdminDTO;

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
        public async Task<UserAdminListDTO> GetUsersByAdminCustom(string? search, bool includeStaff, bool includeCustomers, bool includeHousekeepers, int? pageIndex, int? pageSize)
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
            if(!search.IsNullOrEmpty())
            {
                users = users.Where(c=> (c.FullName.ToLower().Contains(search.ToLower())) || (c.Email.ToLower().Contains(search.ToLower()))).ToList();
            }
            if (pageIndex == null || pageSize == null)
            {
                var temp1 = PaginatedList<UserAdminDTO>.CreateAsync(users, 1, users.Count());
                return new UserAdminListDTO
                {
                    Items = temp1,
                    hasNext = temp1.HasNextPage,
                    hasPrevious = temp1.HasPreviousPage,
                    totalCount = users.Count,
                    totalPages = temp1.TotalPages,
                };
            }
            var temp2 = PaginatedList<UserAdminDTO>.CreateAsync(users, (int)pageIndex, (int)pageSize);
            return new UserAdminListDTO
            {
                Items = temp2,
                hasNext = temp2.HasNextPage,
                hasPrevious = temp2.HasPreviousPage,
                totalCount = users.Count(),
                totalPages = temp2.TotalPages,
            };
        }
    }
}
