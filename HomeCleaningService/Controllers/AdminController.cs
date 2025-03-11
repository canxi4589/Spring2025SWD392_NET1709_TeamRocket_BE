﻿using HCP.Repository.Entities;
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

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IAdminManService _userAdminManService;
        private readonly IAdminServiceService _userAdminServiceService;
        private readonly IAdminManServiceCategory _adminManServiceCategory;

        public AdminController(UserManager<AppUser> userManager, IAdminManService userAdminManService, IAdminServiceService userAdminServiceService, IAdminManServiceCategory adminManServiceCategory)
        {
            _userManager = userManager;
            _userAdminManService = userAdminManService;
            _userAdminServiceService = userAdminServiceService;
            _adminManServiceCategory = adminManServiceCategory;
        }

        [HttpGet("user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersByAdmin([FromQuery] bool includeStaff, [FromQuery] bool includeCustomers, [FromQuery] bool includeHousekeepers)
        {
            var users = await _userAdminManService.GetUsersByAdminCustom(includeStaff, includeCustomers, includeHousekeepers);
            return Ok(new AppResponse<List<UserAdminDTO>>().SetSuccessResponse(users));
        }

        [HttpGet("cleaningService")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllServices(int? pageIndex, int? pageSize)
        {
            var services = await _userAdminServiceService.GetAllServicesAsync(pageIndex, pageSize);
            return Ok(new AppResponse<ServiceAdminShowListDTO>().SetSuccessResponse(services));
        }
        [HttpGet("serviceCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _adminManServiceCategory.GetAllServiceCategoriesAsync();
            return Ok(new AppResponse<List<ServiceCategoryAdminShowDTO>>().SetSuccessResponse(categories));
        }
    }
}
