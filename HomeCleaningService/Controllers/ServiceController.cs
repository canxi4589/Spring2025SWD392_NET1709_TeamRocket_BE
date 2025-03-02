using HCP.Repository.Entities;
using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.Integrations.BlobStorage;
using HCP.Service.Services;
using HCP.Service.Services.CleaningService1;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IBlobStorageService _blobStorageService;
        public ServiceController(ICleaningService1 cleaningService, UserManager<AppUser> userManager, IBlobStorageService blobStorageService)
        {
            _cleaningService = cleaningService;
            _userManager = userManager;
            _blobStorageService = blobStorageService;
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
        [Authorize(Roles = "Staff")]
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

        [HttpPost("uploadMultiple")]
        public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files)
        {
            var response = new AppResponse<List<string>>();

            if (files == null || files.Count == 0)
            {
                return BadRequest(response.SetErrorResponse("Files", "No files uploaded."));
            }

            foreach (var file in files)
            {
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(response.SetErrorResponse("FileSize", $"File {file.FileName} exceeds the size limit of 5 MB."));
                }
            }

            try
            {
                var sasUrls = await _blobStorageService.UploadFilesAsync(files);
                return Ok(response.SetSuccessResponse(sasUrls, "Upload", "Files uploaded successfully!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, response.SetErrorResponse("Exception", ex.Message));
            }
        }


    }
}
