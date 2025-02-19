using HCP.Service.DTOs.CustomerDTO;
using HCP.Service.Services.CustomerService;
using HomeCleaningService.Helpers;
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerById(string id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);

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
        
        [HttpGet("Profile/{id}")]
        public async Task<IActionResult> GetCustomerProfileById(string id)
        {
            var customer = await _customerService.GetCustomerProfileById(id);

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

        [HttpPut("Profile/{id}")]
        public async Task<IActionResult> UpdateCustomerProfile(string id, [FromBody] UpdateCusProfileDto customer)
        {
            var response = new AppResponse<object>();

            try
            {
                await _customerService.UpdateCustomerProfile(id, customer);
                return Ok(response.SetSuccessResponse(null!, "Update", "Profile updated successfully"));
            }
            catch (ArgumentNullException)
            {
                return BadRequest(response.SetErrorResponse("Customer", "Customer ID cannot be null"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, response.SetErrorResponse("Server", $"An error occurred: {ex.Message}"));
            }
        }


    }
}
