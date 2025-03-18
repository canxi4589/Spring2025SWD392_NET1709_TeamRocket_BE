﻿using HCP.DTOs.DTOs.RequestDTO;
using HCP.Repository.Constance;
using HCP.Service.Services.RequestService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<IActionResult> GetAllPendingCreateRequest()
        {
            var response = new AppResponse<string>();

            var result = await _handleRequestService.GetApprovedServiceByStaffIdAsync(User);

            if (result == null)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.Staff, CommonConst.SomethingWrongMessage));
            }

            return Ok(new AppResponse<List<ApprovalServiceDTO>>().SetSuccessResponse(result));
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
