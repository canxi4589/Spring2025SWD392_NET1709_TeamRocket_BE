using HCP.Repository.Constance;
using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.DTOs.RequestDTO;
using HCP.Service.Services.RequestService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IHandleRequestService _handleRequestService;

        public RequestController(IHandleRequestService handleRequestService)
        {
            _handleRequestService = handleRequestService;
        }

        [HttpPut("approve-new-service")]
        [Authorize(Roles = KeyConst.Staff)]
        public async Task<IActionResult> UpdateServiceStatus([FromBody] ServiceStatusUpdateDto dto)
        {
            var response = new AppResponse<string>();

            var result = await _handleRequestService.UpdateServiceStatusAsync(dto, User);

            if (!result.Success)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.PendingRequest, result.Message));
            }

            return Ok(response.SetSuccessResponse(result.Message));
        }

        [HttpGet("service-pending-request")]
        public async Task<IActionResult> GetAllPendingCreateRequest()
        {
            var response = new AppResponse<string>();

            var result = await _handleRequestService.GetPendingCreateServiceRequestsAsync();

            if (result == null)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.PendingRequest, RequestConst.GetRequestNullError));
            }

            return Ok(new AppResponse<List<PendingRequestDTO>>().SetSuccessResponse(result));
        }
        
        [HttpGet("service-pending-request/{id}")]
        public async Task<IActionResult> GetPendingCreateRequestDetail(Guid id)
        {
            var response = new AppResponse<string>();

            var result = await _handleRequestService.GetPendingCreateServiceDetailAsync(id);

            if (result == null)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.PendingRequest, RequestConst.GetRequestNullError));
            }

            return Ok(new AppResponse<PendingRequestDTO>().SetSuccessResponse(result));
        }

    }
}
