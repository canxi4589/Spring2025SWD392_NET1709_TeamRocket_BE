using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.AdminManagementDTO;
using HCP.Service.Services.ListService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using static HCP.Service.DTOs.AdminManagementDTO.ServiceAdminDTO;
using static HCP.Service.DTOs.AdminManagementDTO.ServiceCategoryAdminDTO;

namespace HCP.Service.Services.AdminManService
{
    public class AdminManService : IAdminManService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AdminManService(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
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
        public async Task<ServiceCategoryAdminShowListDTO> GetAllServiceCategoriesAsync(string? search, int? pageIndex, int? pageSize, int? day, int? month, int? year)
        {
            var categories = _unitOfWork.Repository<ServiceCategory>().GetAll();
            var categorieList = categories.Select(category => new ServiceCategoryAdminShowDTO
            {
                CategoryName = category.CategoryName,
                CategoryId = category.Id,
                PictureUrl = category.PictureUrl,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                Description = category.Description
            });
            if (!search.IsNullOrEmpty())
            {
                categorieList = categorieList.Where(c => (c.CategoryName.ToLower().Contains(search.ToLower()))
                || (c.CategoryName.ToLower().Contains(search.ToLower())) || (c.Description.ToLower().Contains(search.ToLower())));
            }
            if (day.HasValue && month.HasValue && year.HasValue)
            {
                categorieList = categorieList.Where(c => (c.CreatedAt.Day == day && c.CreatedAt.Month == month && c.CreatedAt.Year == year));
            }
            if (pageIndex == null || pageSize == null)
            {
                var temp1 = await PaginatedList<ServiceCategoryAdminShowDTO>.CreateAsync(categorieList, 1, categorieList.Count());
                return new ServiceCategoryAdminShowListDTO
                {
                    Items = temp1,
                    hasNext = temp1.HasNextPage,
                    hasPrevious = temp1.HasPreviousPage,
                    totalCount = categorieList.Count(),
                    totalPages = temp1.TotalPages,
                };
            }
            var temp2 = await PaginatedList<ServiceCategoryAdminShowDTO>.CreateAsync(categorieList, (int)pageIndex, (int)pageSize);
            return new ServiceCategoryAdminShowListDTO
            {
                Items = temp2,
                hasNext = temp2.HasNextPage,
                hasPrevious = temp2.HasPreviousPage,
                totalCount = categorieList.Count(),
                totalPages = temp2.TotalPages,
            };
        }
        public async Task<ServiceAdminShowListDTO> GetAllServicesAsync(string? search, int? pageIndex, int? pageSize, int? day, int? month, int? year)
        {
            var services = await _unitOfWork.Repository<CleaningService>()
                .GetAll()
                .Include(c => c.User)
                .Include(c => c.ServiceImages)
                .ToListAsync();

            var svList = new List<ServiceAdminShowDTO>();
            foreach (var service in services)
            {
                var staff = await _userManager.FindByIdAsync(service.StaffId ?? string.Empty);
                svList.Add(new ServiceAdminShowDTO
                {
                    Id = service.Id,
                    ServiceName = service.ServiceName,
                    CategoryId = service.CategoryId,
                    CategoryName = service.Category?.CategoryName ?? "Not found",
                    Status = service.Status,
                    Rating = service.Rating,
                    RatingCount = service.RatingCount,
                    Price = service.Price,
                    City = service.City,
                    District = service.District,
                    AddressLine = service.AddressLine,
                    PlaceId = service.PlaceId,
                    CreatedAt = service.CreatedAt,
                    UpdatedAt = service.UpdatedAt,
                    Duration = service.Duration,
                    HousekeeperId = service.UserId,
                    HousekeeperName = service.User?.FullName ?? "Not found",
                    StaffId = service.StaffId ?? "Not found",
                    StaffName = staff?.FullName ?? "Not found",
                    FirstImgLinkUrl = service.ServiceImages.FirstOrDefault()?.LinkUrl
                });
            }
            if (!search.IsNullOrEmpty())
            {
                svList = svList.Where(c => (c.ServiceName.ToLower().Contains(search.ToLower()))
                || (c.CategoryName.ToLower().Contains(search.ToLower()))
                || (c.City.ToLower().Contains(search.ToLower()))
                || (c.District.ToLower().Contains(search.ToLower()))
                || (c.AddressLine.ToLower().Contains(search.ToLower()))
                || (c.HousekeeperName.ToLower().Contains(search.ToLower()))
                || (c.StaffName.ToLower().Contains(search.ToLower()))
                || (c.HousekeeperName.ToLower().Contains(search.ToLower()))
                || (c.Price.ToString().ToLower().Contains(search.ToLower()))
                ).ToList();
            }
            if (day.HasValue && month.HasValue && year.HasValue)
            {
                svList = svList.Where(c => (c.CreatedAt.Day == day && c.CreatedAt.Month == month && c.CreatedAt.Year == year)).ToList();
            }
            if (pageIndex == null || pageSize == null)
            {
                var temp1 = PaginatedList<ServiceAdminShowDTO>.Create(svList.AsQueryable(), 1, svList.Count);
                return new ServiceAdminShowListDTO
                {
                    Items = temp1,
                    hasNext = temp1.HasNextPage,
                    hasPrevious = temp1.HasPreviousPage,
                    totalCount = svList.Count,
                    totalPages = temp1.TotalPages,
                };
            }
            var temp2 = PaginatedList<ServiceAdminShowDTO>.Create(svList.AsQueryable(), (int)pageIndex, (int)pageSize);
            return new ServiceAdminShowListDTO
            {
                Items = temp2,
                hasNext = temp2.HasNextPage,
                hasPrevious = temp2.HasPreviousPage,
                totalCount = svList.Count,
                totalPages = temp2.TotalPages,
            };
        }
        public async Task<ServiceAdminShowListDTO> GetRevenueChartDatas(bool monthChart, bool weekChart, bool yearChart, bool yearsChart, int? dayStart, int? monthStart, int? yearStart, int? dayEnd, int? monthEnd, int? yearEnd)
        {

        }
    }
}
