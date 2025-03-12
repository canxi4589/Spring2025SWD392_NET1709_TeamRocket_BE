using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.Integrations.BlobStorage;
using HCP.Service.Services;
using HCP.Service.Services.CleaningService1;
using HCP.Service.Services.CustomerService;
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
        private readonly ICustomerService _customerService;
        public ServiceController(ICleaningService1 cleaningService, UserManager<AppUser> userManager, IBlobStorageService blobStorageService, ICustomerService customerService)
        {
            _cleaningService = cleaningService;
            _userManager = userManager;
            _blobStorageService = blobStorageService;
            _customerService = customerService;
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
                return NotFound(new AppResponse<ServiceDetailDTO>().SetErrorResponse(KeyConst.CleaningService, CleaningServiceConst.ServiceNotFound));

            return Ok(new AppResponse<ServiceDetailDTO>().SetSuccessResponse(service));
        }
        
        [HttpGet("user")]
        public async Task<IActionResult> GetServiceByUser()
        {
            var service = await _cleaningService.GetServiceByUser(User);

            if (service == null)
                return NotFound(new AppResponse<ServiceDetailWithStatusDTO>().SetErrorResponse(KeyConst.CleaningService,CleaningServiceConst.ServiceNotFound));

            return Ok(new AppResponse<List<ServiceDetailWithStatusDTO>>().SetSuccessResponse(service));
        }
        
        [HttpGet("user/filter")]
        [Authorize]
        public async Task<IActionResult> GetServiceByUser(string? status, int? pageIndex, int? pageSize)
        {
            var service = await _cleaningService.GetServiceByUserFilter(status, User, pageIndex, pageSize);

            if (service == null)
                return NotFound(new AppResponse<ServiceOverviewListDTO>().SetErrorResponse(KeyConst.CleaningService,CleaningServiceConst.ServiceNotFound));

            return Ok(new AppResponse<ServiceOverviewListDTO>().SetSuccessResponse(service));
        }
        
        [HttpGet("detail/{id}")]
        [Authorize]
        public async Task<IActionResult> GetServiceDetail(Guid id)
        {
            var service = await _cleaningService.GetCleaningServiceDetailAsync(id, User);

            if (service == null)
                return NotFound(new AppResponse<CreateCleaningServiceDTO>().SetErrorResponse(KeyConst.CleaningService,CleaningServiceConst.ServiceNotFound));

            return Ok(new AppResponse<CreateCleaningServiceDTO>().SetSuccessResponse(service));
        }

        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> CreateCleaningService([FromBody] CreateCleaningServiceDTO dto)
        {
            var createdService = await _cleaningService.CreateCleaningServiceAsync(dto, User);

            if (createdService == null)
            {
                var errorResponse = new AppResponse<object>()
                    .SetErrorResponse(KeyConst.CleaningService, CleaningServiceConst.AddError);
                return BadRequest(errorResponse);
            }

            var successResponse = new AppResponse<object>()
                .SetSuccessResponse(createdService);
            return Ok(successResponse);
        }


        [HttpPut("{serviceId}")]
        [Authorize]
        public async Task<IActionResult> UpdateCleaningService(Guid serviceId, [FromBody] CreateCleaningServiceDTO dto)
        {
            var updatedService = await _cleaningService.UpdateCleaningServiceAsync(serviceId, dto, User);

            if (updatedService == null)
            {
                var errorResponse = new AppResponse<object>()
                    .SetErrorResponse(KeyConst.CleaningService, CommonConst.SomethingWrongMessage);
                return BadRequest(errorResponse);
            }

            var successResponse = new AppResponse<object>()
                .SetSuccessResponse(updatedService);
            return Ok(successResponse);
        }

        [HttpPost("uploadMultiple")]
        public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files)
        {
            var response = new AppResponse<List<string>>();

            if (files == null || files.Count == 0)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.File, CommonConst.SomethingWrongMessage));
            }

            foreach (var file in files)
            {
                if (file.Length > 5 * 1024 * 1024)
                {
                    return BadRequest(response.SetErrorResponse(KeyConst.FileSize, CommonConst.FileHandleError));
                }
            }

            try
            {
                var sasUrls = await _blobStorageService.UploadFilesAsync(files);
                return Ok(response.SetSuccessResponse(sasUrls, KeyConst.Upload, CommonConst.SuccessTaskMessage));
            }
            catch (Exception ex)
            {
                return StatusCode(500, response.SetErrorResponse(KeyConst.Exception, ex.Message));
            }
        }

        [HttpPost("GetTimeSlots")]
        [Authorize]
        public async Task<IActionResult> GetServiceTimeSlotByDay([FromBody] TimeSLotRequest dto)

        {
            var list = await _cleaningService.GetAllServiceTimeSlot(dto.serviceId,dto.targetDate,dto.dayOfWeek);
            var successResponse = new AppResponse<object>()
            .SetSuccessResponse(list);
            return Ok(successResponse);
        }

        [HttpGet("GetAllAdditionals")]
        [Authorize]
        public async Task<IActionResult> GetAllAdditionalServices( Guid serviceId)
        {
            var list = await _cleaningService.GetAllAdditonalServicesById(serviceId);
            var successResponse = new AppResponse<object>()
            .SetSuccessResponse(list);
            return Ok(successResponse);
        }

        [HttpGet("GetCustomerProfileInCheckout")]
        [Authorize]
        public async Task<IActionResult> GetCusProfile()
        {
            var list = await _customerService.GetCustomerCheckoutProfile(User);
            var successResponse = new AppResponse<object>()
            .SetSuccessResponse(list);
            return Ok(successResponse);
        }
    }
}
