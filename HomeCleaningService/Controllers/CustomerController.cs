using HCP.DTOs.DTOs.CustomerDTO;
using HCP.Repository.Constance;
using HCP.Service.Services.CustomerService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetCustomer()
        {
            var customer = await _customerService.GetCustomerAsync(User);

            if (customer == null)
            {
                var errorResponse = new AppResponse<object>()
                    .SetErrorResponse(KeyConst.Customer, CustomerConst.NotFoundError);
                return NotFound(errorResponse);
            }

            var successResponse = new AppResponse<object>()
                .SetSuccessResponse(customer);
            return Ok(successResponse);
        }
        
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetCustomerProfile()
        {
            var customer = await _customerService.GetCustomerProfile(User);

            if (customer == null)
            {
                var errorResponse = new AppResponse<object>()
                    .SetErrorResponse(KeyConst.Customer, CustomerConst.ProfileNotFoundError);
                return NotFound(errorResponse);
            }

            var successResponse = new AppResponse<object>()
                .SetSuccessResponse(customer);
            return Ok(successResponse);
        }

        [HttpPut("profile")]
        [Authorize] 
        public async Task<IActionResult> UpdateCustomerProfile([FromBody] UpdateCusProfileDto customer)
        {
            var response = new AppResponse<object>();

            try
            {
                var updateUser = await _customerService.UpdateCustomerProfile(customer, User);
                return Ok(response.SetSuccessResponse(updateUser, KeyConst.Customer, CustomerConst.UpdateSuccess));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(response.SetErrorResponse(KeyConst.Unathorized, CommonConst.UnauthorizeError));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(response.SetErrorResponse(KeyConst.Customer, CustomerConst.NotFoundError));
            }
            catch (Exception ex)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.Customer, ex.Message));
            }
        }
        [HttpPut("profileAvatar")]
        [Authorize] 
        public async Task<IActionResult> UpdateCustomerAvatarProfile(string avatar)
        {
            var response = new AppResponse<object>();

            try
            {
                var updateUserAvatar = await _customerService.UpdateCustomerAvatarProfile(avatar, User);
                return Ok(response.SetSuccessResponse(updateUserAvatar, KeyConst.Customer, CustomerConst.UpdateSuccess));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(response.SetErrorResponse(KeyConst.Unathorized, CommonConst.UnauthorizeError));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(response.SetErrorResponse(KeyConst.Customer, CustomerConst.NotFoundError));
            }
            catch (Exception ex)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.Customer, ex.Message));
            }
        }


    }
}
