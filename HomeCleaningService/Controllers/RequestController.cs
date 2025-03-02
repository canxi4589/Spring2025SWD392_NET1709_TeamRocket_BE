using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.DTOs.RequestDTO;
using HCP.Service.Services.RequestService;
using HomeCleaningService.Helpers;
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
        public async Task<IActionResult> UpdateServiceStatus([FromBody] ServiceStatusUpdateDto dto)
        {
            var response = new AppResponse<string>();

            var result = await _handleRequestService.UpdateServiceStatusAsync(dto);

            if (!result.Success)
            {
                return BadRequest(response.SetErrorResponse("error", result.Message));
            }

            return Ok(response.SetSuccessResponse(result.Message));
        }

        [HttpGet("pending-request")]
        public async Task<IActionResult> GetAllPendingCreateRequest()
        {
            var response = new AppResponse<string>();

            var result = await _handleRequestService.GetPendingCreateServiceRequestsAsync();

            if (result == null)
            {
                return BadRequest(response.SetErrorResponse("error", "Something wrong getting Pending Request"));
            }

            return Ok(new AppResponse<List<PendingRequestDTO>>().SetSuccessResponse(result));
        }
    }
}
