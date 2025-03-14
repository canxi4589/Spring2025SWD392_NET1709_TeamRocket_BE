using HCP.Repository.Entities;
using HCP.Service.DTOs.CustomerDTO;
using HCP.Service.DTOs.AdminManagementDTO;
using HCP.Service.Services.AdminManService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using static HCP.Service.DTOs.AdminManagementDTO.ServiceAdminDTO;
using HCP.Service.DTOs.CleaningServiceDTO;
using static HCP.Service.DTOs.AdminManagementDTO.ServiceCategoryAdminDTO;
using HCP.Repository.Constance;
using static HCP.Service.DTOs.AdminManagementDTO.ChartDataAdminDTO;

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAdminManService _adminManService;

        public AdminController(UserManager<AppUser> userManager, IAdminManService adminManService)
        {
            _userManager = userManager;
            _adminManService = adminManService;
        }

        [HttpGet("user")]
        [Authorize(Roles = KeyConst.Admin)]
        public async Task<IActionResult> GetUsersByAdmin(string? search, bool includeStaff, bool includeCustomers, bool includeHousekeepers, int? pageIndex, int? pageSize)
        {
            var users = await _adminManService.GetUsersByAdminCustom(search, includeStaff, includeCustomers, includeHousekeepers, pageIndex, pageSize);
            return Ok(new AppResponse<UserAdminListDTO>().SetSuccessResponse(users));
        }

        [HttpGet("cleaningService")]
        [Authorize(Roles = KeyConst.Admin)]
        public async Task<IActionResult> GetAllServices(string? search, int? pageIndex, int? pageSize, int? day, int? month, int? year)
        {
            var services = await _adminManService.GetAllServicesAsync(search, pageIndex, pageSize, day, month, year);
            return Ok(new AppResponse<ServiceAdminShowListDTO>().SetSuccessResponse(services));
        }
        [HttpGet("serviceCategory")]
        [Authorize(Roles = KeyConst.Admin)]
        public async Task<IActionResult> GetAllCategories(string? search, int? pageIndex, int? pageSize, int? day, int? month, int? year)
        {
            var categories = await _adminManService.GetAllServiceCategoriesAsync(search, pageIndex, pageSize, day, month, year);
            return Ok(new AppResponse<ServiceCategoryAdminShowListDTO>().SetSuccessResponse(categories));
        }
        [HttpGet("revenueChartData")]
        [Authorize(Roles = KeyConst.Admin)]
        public async Task<IActionResult> GetRevenueChartData(bool dayChart, bool weekChart, bool yearChart, bool yearsChart, int? dayStart, int? monthStart, int? yearStart, int? dayEnd, int? monthEnd, int? yearEnd)
        {
            var chartData = await _adminManService.GetRevenueChartDatas(dayChart, weekChart, yearChart, yearsChart, dayStart, monthStart, yearStart, dayEnd, monthEnd, yearEnd);
            return Ok(new AppResponse<ChartDataAdminShowListDTO>().SetSuccessResponse(chartData));
        }
    }
}
