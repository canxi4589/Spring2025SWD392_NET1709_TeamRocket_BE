using HCP.DTOs.DTOs.AdminManagementDTO;
using HCP.DTOs.DTOs.BookingDTO;
using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Service.Services.AdminManService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static HCP.DTOs.DTOs.AdminManagementDTO.ChartDataAdminDTO;
using static HCP.DTOs.DTOs.AdminManagementDTO.ServiceAdminDTO;
using static HCP.DTOs.DTOs.AdminManagementDTO.ServiceCategoryAdminDTO;

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
        [HttpGet("serviceCategoryChart")]
        [Authorize(Roles = KeyConst.Admin)]
        public async Task<IActionResult> GetCategoriesChart(bool dayChart, bool yearChart, bool yearsChart, int? dayStart, int? monthStart, int? yearStart, int? dayEnd, int? monthEnd, int? yearEnd)
        {
            var categories = await _adminManService.GetCategoryChart(dayChart, yearChart, yearsChart, dayStart, monthStart, yearStart, dayEnd, monthEnd, yearEnd);
            return Ok(new AppResponse<ChartCategoryDataAdminShowListDTO>().SetSuccessResponse(categories));
        }
        [HttpGet("revenueChartData")]
        [Authorize(Roles = KeyConst.Admin)]
        public async Task<IActionResult> GetRevenueChartData(bool dayChart, bool weekChart, bool yearChart, bool yearsChart, int? dayStart, int? monthStart, int? yearStart, int? dayEnd, int? monthEnd, int? yearEnd)
        {
            var chartData = await _adminManService.GetRevenueChartDatas(dayChart, weekChart, yearChart, yearsChart, dayStart, monthStart, yearStart, dayEnd, monthEnd, yearEnd);
            return Ok(new AppResponse<ChartDataAdminShowListDTO>().SetSuccessResponse(chartData));
        }
        [HttpGet("bookingsAdminView")]
        [Authorize(Roles = KeyConst.Admin)]
        public async Task<IActionResult> GetAllBookingByCateAndService(bool isService, bool isCategory, Guid Id, int? pageIndex, int? pageSize, string? status, int? day, int? month, int? year)
        {
            var list = await _adminManService.GetAllBookingByCateAndServiceAdmin(isService, isCategory, Id, pageIndex, pageSize, status, day, month, year);
            return Ok(new AppResponse<BookingHistoryResponseListDTO>().SetSuccessResponse(list));
        }
    }
}
