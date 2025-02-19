using HCP.Service.DTOs.CustomerDTO;
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
            var customer = await _customerService.GetCustomerByIdAsync(User);

            if (customer == null)
            {
                var errorResponse = new AppResponse<object>()
                    .SetErrorResponse("Customer", "Customer not found");
                return NotFound(errorResponse);
            }

            var successResponse = new AppResponse<object>()
                .SetSuccessResponse(customer);
            return Ok(successResponse);
        }
        
        [HttpGet("Profile")]
        [Authorize]
        public async Task<IActionResult> GetCustomerProfile()
        {
            var customer = await _customerService.GetCustomerProfileById(User);

            if (customer == null)
            {
                var errorResponse = new AppResponse<object>()
                    .SetErrorResponse("Customer Profile", "Customer Profile not found");
                return NotFound(errorResponse);
            }

            var successResponse = new AppResponse<object>()
                .SetSuccessResponse(customer);
            return Ok(successResponse);
        }

        [HttpPut("Profile")]
        [Authorize] 
        public async Task<IActionResult> UpdateCustomerProfile([FromBody] UpdateCusProfileDto customer)
        {
            var response = new AppResponse<object>();

            try
            {
                var updateUser = await _customerService.UpdateCustomerProfile(customer, User);
                return Ok(response.SetSuccessResponse(updateUser, "Update", "Profile updated successfully"));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(response.SetErrorResponse("Authentication", "User not authenticated"));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(response.SetErrorResponse("Customer", "User not found"));
            }
            catch (Exception ex)
            {
                return BadRequest(response.SetErrorResponse("Update", ex.Message));
            }
        }



    }
}
