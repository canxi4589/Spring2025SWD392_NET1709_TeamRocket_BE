using HCP.Repository.Entities;
using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.Services.CleaningService1;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly ICleaningService1 _cleaningService;
        private readonly UserManager<AppUser> _userManager;
        public ServiceController(ICleaningService1 cleaningService, UserManager<AppUser> userManager)
        {
            _cleaningService = cleaningService;
            _userManager = userManager;
        }
        [HttpGet("Categories")]
        public async Task<IActionResult> getAllCategories()
        {
            var list =await _cleaningService.GetAllCategories();
            return Ok(new AppResponse<List<CategoryDTO>>()
            .SetSuccessResponse(list));
        }
    }
}
