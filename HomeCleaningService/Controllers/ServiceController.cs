using HCP.Repository.Entities;
using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.Services.CleaningService1;
using HCP.Service.Services.ListService;
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
            var list =await _cleaningService.GetAllCategories(1,8);
            return Ok(new AppResponse<List<CategoryDTO>>()
            .SetSuccessResponse(list));
        }
        
        [HttpGet()]
        public async Task<IActionResult> getAllServices1(int? pageIndex, int? pageSize)
        {

            var list = await _cleaningService.GetAllServiceItems(pageIndex,pageSize);
            return Ok(new AppResponse<CleaningServiceListDTO>()
            .SetSuccessResponse(list));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(Guid id)
        {
            var service = await _cleaningService.GetServiceById(id);

            if (service == null)
                return NotFound(new AppResponse<ServiceDetailDTO>().SetErrorResponse("Error", "Service not found"));

            return Ok(new AppResponse<ServiceDetailDTO>().SetSuccessResponse(service));
        }

    }
}
