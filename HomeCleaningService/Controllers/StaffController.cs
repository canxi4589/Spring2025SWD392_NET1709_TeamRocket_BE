using HCP.DTOs.DTOs.AdminManagementDTO;
using HCP.DTOs.DTOs.RequestDTO;
using HCP.Repository.Constance;
using HCP.Service.Services.AdminManService;
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
    public class StaffController : ControllerBase
    {
        private readonly IHandleRequestService _handleRequestService;
        private readonly IAdminManService _adminService;

        public StaffController(IHandleRequestService handleRequestService, IAdminManService adminService)
        {
            _handleRequestService = handleRequestService;
            _adminService = adminService;
        }

        [HttpGet("staff-requests-approved")]
        [Authorize(Roles = KeyConst.Staff)]
        public async Task<IActionResult> GetAllApprovalCreateRequest()
        {
            var response = new AppResponse<string>();

            var result = await _handleRequestService.GetApprovedServiceByStaffIdAsync(User);

            if (result == null)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.Staff, CommonConst.SomethingWrongMessage));
            }

            return Ok(new AppResponse<List<ApprovalServiceDTO>>().SetSuccessResponse(result));
        }
        
        [HttpGet("staff-requests-approved-paging")]
        [Authorize(Roles = KeyConst.Staff)]
        public async Task<IActionResult> GetAllApprovalCreateRequestFilter(int? pageIndex, int? pageSize, string? status, string? searchByName)
        {
            var response = new AppResponse<string>();

            var result = await _handleRequestService.GetAllApprovedServiceByStaffAsync(User, pageIndex, pageSize, status, searchByName);

            if (result == null)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.Staff, CommonConst.SomethingWrongMessage));
            }

            return Ok(new AppResponse<ApprovalServiceListDTO>().SetSuccessResponse(result));
        }

        [HttpGet("staff-approval-registration")]
        [Authorize(Roles = KeyConst.Staff)]
        public async Task<IActionResult> GetApprovalRegistrationByStaff(int? pageIndex, int? pageSize, string? status)
        {
            var response = new AppResponse<string>();
            try
            {
                var result = await _handleRequestService.GetStaffRegistrationApproval(User, pageIndex, pageSize, status);

                return Ok(new AppResponse<RegistrationRequestListDTO>().SetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.Error, ex.ToString()));
            }
        }

        [HttpPut("staff-change-password")]
        [Authorize(Roles = KeyConst.Staff)]
        public async Task<IActionResult> CreateStaff(ChangPasswordStaffDTO request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToArray();
                return BadRequest(new AppResponse<object>().SetErrorResponse(KeyConst.ModelState, errors));
            }
            try
            {
                await _adminService.ChangeTemporalPassword(request, User);
                return Ok(new AppResponse<object>().SetSuccessResponse(AuthenticationConst.ChangPasswordSuccess));
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new AppResponse<object>().SetErrorResponse(KeyConst.Validate, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(new AppResponse<object>().SetErrorResponse(KeyConst.Error, ex.Message));
            }
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
