using HCP.Repository.Entities;
using HCP.Service.DTOs.AdminManagementDTO;
using HCP.Service.Services.ListService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<UserAdminListDTO> GetUsersByAdminCustom([FromQuery] bool includeStaff, [FromQuery] bool includeCustomers, [FromQuery] bool includeHousekeepers, int? pageIndex, int? pageSize)
        {
            var userss = await _userManager.GetUsersInRoleAsync("Staff");
            if (!includeStaff)
            {
                userss = userss.Where(user => user != null).ToList();
            }
            if (includeCustomers)
            {
                var cus = await _userManager.GetUsersInRoleAsync("Customer");
                foreach (var item in cus)
                {
                    userss.Add(item);
                }
            }
            if (includeHousekeepers)
            {
                var hkp = await _userManager.GetUsersInRoleAsync("Housekeeper");
                foreach (var item in hkp)
                {
                    userss.Add(item);
                }
            }
            var userList = userss.Select(user => new UserAdminDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty, // Handle possible null reference
                PhoneNumber = user.PhoneNumber ?? string.Empty, // Handle possible null reference
                Status = "IsActive",
                Birthday = user.Birthday,
                Avatar = user.Avatar
            }).AsQueryable(); // Convert to IQueryable

            if (pageIndex == null || pageSize == null)
            {
                var temp1 = await PaginatedList<UserAdminDTO>.CreateAsync(userList, 1, userList.Count());
                return new UserAdminListDTO
                {
                    Items = temp1,
                    hasNext = temp1.HasNextPage,
                    hasPrevious = temp1.HasPreviousPage,
                    totalCount = userss.Count,
                    totalPages = temp1.TotalPages,
                };
            }
            var temp2 = await PaginatedList<UserAdminDTO>.CreateAsync(userList, (int)pageIndex, (int)pageSize);
            return new UserAdminListDTO
            {
                Items = temp2,
                hasNext = temp2.HasNextPage,
                hasPrevious = temp2.HasPreviousPage,
                totalCount = userList.Count(),
                totalPages = temp2.TotalPages,
            };
        }
    }
}
