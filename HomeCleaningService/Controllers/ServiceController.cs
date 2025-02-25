using HCP.Repository.Entities;
using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.Services.CleaningService1;
using HomeCleaningService.Helpers;
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
        
        [HttpGet("user")]
        public async Task<IActionResult> GetServiceByUser()
        {
            var service = await _cleaningService.GetServiceByUser(User);

            if (service == null)
                return NotFound(new AppResponse<ServiceDetailWithStatusDTO>().SetErrorResponse("Error", "Service not found"));

            return Ok(new AppResponse<List<ServiceDetailWithStatusDTO>>().SetSuccessResponse(service));
        }

        [HttpPost()]
        public async Task<IActionResult> CreateCleaningService([FromBody] CreateCleaningServiceDTO dto)
        {
            var createdService = await _cleaningService.CreateCleaningServiceAsync(dto, User);

            if (createdService == null)
            {
                var errorResponse = new AppResponse<object>()
                    .SetErrorResponse("Cleaning Service", "Failed to create cleaning service.");
                return BadRequest(errorResponse);
            }

            var successResponse = new AppResponse<object>()
                .SetSuccessResponse(createdService);
            return Ok(successResponse);
        }

    }
}
