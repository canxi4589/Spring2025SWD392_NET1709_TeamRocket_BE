using HCP.DTOs.DTOs.CleaningServiceDTO;
using HCP.DTOs.DTOs.RequestDTO;
using HCP.Repository.Constance;
using HCP.Service.Services.RequestService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet("registraton-requests")]
        public async Task<IActionResult> GetPendingHousekeeperRegistrationRequestsAsync(int? pageIndex, int? pageSize)
        {
            var response = new AppResponse<string>();

            var result = await _handleRequestService.GetPendingHousekeeperRegistrationRequestsAsync(pageIndex, pageSize);

            if (result == null)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.PendingRequest, RequestConst.GetRequestNullError));
            }

            return Ok(new AppResponse<RegistrationRequestListDTO>().SetSuccessResponse(result));
        }

        [HttpGet("registraton-request-detail")]
        public async Task<IActionResult> GetPendingHousekeeperRegistrationRequestDetailAsync(string housekeeperId)
        {
            var response = new AppResponse<string>();
            try
            {
                var result = await _handleRequestService.GetPendingHousekeeperRegistrationRequestDetailAsync(User, housekeeperId);

                if (result == null)
                {
                    return BadRequest(response.SetErrorResponse(KeyConst.PendingRequest, RequestConst.GetRequestNullError));
                }

                return Ok(new AppResponse<RegistrationRequestDetailDTO>().SetSuccessResponse(result));
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.PendingRequest, ex.ToString()));
            }
        }

        [HttpPut("approve-registration")]
        [Authorize(Roles = KeyConst.Staff)]
        public async Task<IActionResult> UpdateHouskeeperVerificationStatusAsync(RegistrationRequestStatusUpdateDto dto)
        {
            var response = new AppResponse<string>();

            var result = await _handleRequestService.UpdateHouskeeperVerificationStatusAsync(dto, User);

            if (!result.Success)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.PendingRequest, result.Message));
            }

            return Ok(response.SetSuccessResponse(result.Message));
        }

    }
}
