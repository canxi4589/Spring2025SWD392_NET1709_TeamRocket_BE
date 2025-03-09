using HCP.Service.DTOs.RequestDTO;
using HCP.Service.Services.RequestService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffController : ControllerBase
    {
        private readonly IHandleRequestService _handleRequestService;

        public StaffController(IHandleRequestService handleRequestService)
        {
            _handleRequestService = handleRequestService;
        }

        [HttpGet("staff-requests-approved")]
        public async Task<IActionResult> GetAllPendingCreateRequest()
        {
            var response = new AppResponse<string>();

            var result = await _handleRequestService.GetApprovedServiceByStaffIdAsync(User);

            if (result == null)
            {
                return BadRequest(response.SetErrorResponse("error", "Something wrong getting History Request of Staff"));
            }

            return Ok(new AppResponse<List<ApprovalServiceDTO>>().SetSuccessResponse(result));
        }

        //[HttpGet("staff-requests-approved/{id}")]
        //public async Task<IActionResult> GetPendingCreateRequestDetail(Guid id)
        //{
        //    var response = new AppResponse<string>();

        //    var result = await _handleRequestService.GetPendingCreateServiceDetailAsync(id);

        //    if (result == null)
        //    {
        //        return BadRequest(response.SetErrorResponse("error", "Something wrong getting Pending Request"));
        //    }

        //    return Ok(new AppResponse<PendingRequestDTO>().SetSuccessResponse(result));
        //}

    }
}
