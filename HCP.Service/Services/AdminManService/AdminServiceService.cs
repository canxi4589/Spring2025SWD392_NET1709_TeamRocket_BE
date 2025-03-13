using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.BookingDTO;
using HCP.Service.Services.CustomerService;
using HCP.Service.Services.ListService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HCP.Service.DTOs.AdminManagementDTO.ServiceAdminDTO;

namespace HCP.Service.Services.AdminManService
{
    public class AdminServiceService : IAdminServiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerService _customerService;
        private readonly UserManager<AppUser> _userManager;

        public AdminServiceService(IUnitOfWork unitOfWork, ICustomerService customerService, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _customerService = customerService;
            _userManager = userManager;
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
            if(!search.IsNullOrEmpty())
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
    }
}
