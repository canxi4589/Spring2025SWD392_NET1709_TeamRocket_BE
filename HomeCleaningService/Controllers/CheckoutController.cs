using HCP.DTOs.DTOs.CheckoutDTO;
using HCP.Repository.Constance;
using HCP.Service.Services.CheckoutService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
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
                    var errorResponse = new AppResponse<object>()
                        .SetErrorResponse(KeyConst.Checkout, CheckoutConst.CreateCheckoutError);
                    return BadRequest(errorResponse);
                }

                var successResponse = new AppResponse<object>()
                    .SetSuccessResponse(createdCheckout);
                return Ok(successResponse);
            }
            catch (DbUpdateException dbEx)
            {
                var errorResponse = new AppResponse<object>()
                    .SetErrorResponse(KeyConst.Error, dbEx.Message);
                return StatusCode(500, errorResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new AppResponse<object>()
                    .SetErrorResponse(KeyConst.Error, ex.Message);
                return StatusCode(400, errorResponse);
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
