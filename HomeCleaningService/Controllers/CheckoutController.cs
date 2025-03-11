using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Service.DTOs.BookingDTO;
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
                    return BadRequest(new { error = CheckoutConst.CreateCheckoutError });
                }

                return Ok(new { message = CheckoutConst.CreateCheckoutSuccess, data = createdCheckout });
            }
            catch (DbUpdateException dbEx)
            {
                //Console.WriteLine($"Database Error: {dbEx.Message} | Inner: {dbEx.InnerException?.Message}");
                return StatusCode(500, new { error = CommonConst.DatabaseError, details = dbEx.Message });
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Unhandled Exception: {ex.Message}");
                return StatusCode(500, new { error = CommonConst.InternalError, details = ex.Message });
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
                    .SetErrorResponse(KeyConst.Checkout, CheckoutConst.UpdateCheckoutError);
                return NotFound(errorResponse);
            }

            var successResponse = new AppResponse<object>()
                .SetSuccessResponse(CheckoutConst.UpdateCheckoutSuccess);
            return Ok(successResponse);
        }
        
        [HttpGet("{checkoutId}")]
        [Authorize]
        public async Task<IActionResult> GetCheckoutById(Guid checkoutId)
        {
            var checkoutResponse = await _checkoutService.GetCheckoutById(checkoutId);

            if (checkoutResponse == null)
            {
                var errorResponse = new AppResponse<object>()
                    .SetErrorResponse(KeyConst.Checkout, CheckoutConst.GetCheckoutNullError);
                return NotFound(errorResponse);
            }

            var successResponse = new AppResponse<CheckoutResponseDTO1>()
                .SetSuccessResponse(checkoutResponse);
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
                    .SetErrorResponse(KeyConst.Checkout, CheckoutConst.GetCheckoutNullError);
                return NotFound(errorResponse);
            }

            var successResponse = new AppResponse<List<CheckoutResponseDTO1>>()
                .SetSuccessResponse(pendingCheckouts);
            return Ok(successResponse);
        }
    }
}
