using HCP.Repository.Entities;
using HCP.Service.DTOs.CheckoutDTO;
using HCP.Service.Services.CheckoutService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : ControllerBase
    {
        private readonly ICheckoutService _checkoutService;

        public CheckoutController(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateCheckout([FromBody] CheckoutRequestDTO1 requestDTO)
        {
            try
            {
                var createdCheckout = await _checkoutService.CreateCheckout(requestDTO, User);

                if (createdCheckout == null)
                {
                    return BadRequest(new { error = "Failed to create checkout." });
                }

                return Ok(new { message = "Checkout created successfully!", data = createdCheckout });
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database Error: {dbEx.Message} | Inner: {dbEx.InnerException?.Message}");
                return StatusCode(500, new { error = "Database error occurred.", details = dbEx.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled Exception: {ex.Message}");
                return StatusCode(500, new { error = "Internal Server Error", details = ex.Message });
            }
        }



        [HttpPut("change-status/{checkoutId}")]
        [Authorize]
        public async Task<IActionResult> ChangeStatusCheckout(Guid checkoutId)
        {
            var isUpdated = await _checkoutService.ChangeStatusCheckout(checkoutId);

            if (!isUpdated)
            {
                var errorResponse = new AppResponse<object>()
                    .SetErrorResponse("Checkout", "Failed to change checkout status. Checkout may not exist.");
                return NotFound(errorResponse);
            }

            var successResponse = new AppResponse<object>()
                .SetSuccessResponse("Checkout status updated successfully.");
            return Ok(successResponse);
        }

        [HttpGet("pending-checkout")]
        [Authorize]
        public async Task<IActionResult> GetPendingCheckouts()
        {
            var pendingCheckouts = await _checkoutService.GetPendingCheckouts(User);

            if (!pendingCheckouts.Any())
            {
                var errorResponse = new AppResponse<object>()
                    .SetErrorResponse("Checkout", "No pending checkouts found.");
                return NotFound(errorResponse);
            }

            var successResponse = new AppResponse<List<CheckoutResponseDTO1>>()
                .SetSuccessResponse(pendingCheckouts);
            return Ok(successResponse);
        }
    }
}
