using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.BookingDTO;
using HCP.Service.DTOs.CustomerDTO;
using HCP.Service.DTOs.WalletDTO;
using HCP.Service.Services.BookingService;
using HCP.Service.Services.CustomerService;
using HCP.Service.Services.ListService;
using HCP.Service.Services.TemporaryService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingService _bookingService;
        private readonly ICustomerService _customerService;
        private readonly ITemporaryStorage _temporaryStorage;

        public BookingController(IUnitOfWork unitOfWork, IBookingService bookingService, ICustomerService customerService, ITemporaryStorage temporaryStorage)
        {
            _unitOfWork = unitOfWork;
            _bookingService = bookingService;
            _customerService = customerService;
            _temporaryStorage = temporaryStorage;
        }


        [HttpGet("bookingHistory")]
        [Authorize]
        public async Task<IActionResult> GetAllBookingByUser(int? pageIndex, int? pageSize, string? status, int? day, int? month, int? year)
        {
            var user = await _customerService.GetCustomerAsync(User);
            if (user != null)
            {
                var list = await _bookingService.GetBookingByUser(user, pageIndex, pageSize, status, day, month, year);
                return Ok(new AppResponse<BookingHistoryResponseListDTO>().SetSuccessResponse(list));
            }
            return NotFound();
        }
        [HttpGet("bookingDetail")]
        [Authorize]
        public async Task<IActionResult> GetBookingDetailById(Guid id)
        {
            var booking = await _unitOfWork.Repository<Booking>().FindAsync(c => c.Id == id);
            var user = await _customerService.GetCustomerAsync(User);
            if (user != null)
            {
                var bookingDetail = await _bookingService.GetBookingDetailById(booking);
                if (bookingDetail != null)
                {
                    return Ok(new AppResponse<BookingHistoryDetailResponseDTO>().SetSuccessResponse(bookingDetail));
                }
            }
            return NotFound();
        }
        [HttpGet("bookingCountHousekeeper")]
        [Authorize]
        public async Task<IActionResult> GetBookingCountByHousekeeper()
        {
            var bookingCount = await _bookingService.GetBookingCountHousekeeper(User);
            return Ok(new AppResponse<BookingCountDTO>().SetSuccessResponse(bookingCount));
        }
        [HttpGet("GetHousekeeperBookings")]
        [Authorize]
        public async Task<IActionResult> GetHousekeeperBookings([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null)
        {
            var userClaims = User;
            var response = new AppResponse<PaginatedList<BookingListItemDto>>();
            try
            {
                var result = await _bookingService.GetHousekeeperBookingsAsync(userClaims, page, pageSize, status);
                return Ok(response.SetSuccessResponse(result));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(response.SetErrorResponse(KeyConst.Unathorized, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.Error, ex.Message));
            }
        }
        [HttpDelete("cancelBooking")]
        [Authorize]
        public async Task<IActionResult> cancelBooking(Guid bookingId)
        {
            var user = await _customerService.GetCustomerAsync(User);
            if (user != null)
            {
                var cancelBooking = await _bookingService.cancelBooking(bookingId, user);
                return Ok(new AppResponse<BookingCancelDTO>().SetSuccessResponse(cancelBooking));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse(KeyConst.Error, TransactionConst.RefundFail));
        }
        [HttpPost("submit-proof")]
        [Authorize]
        public async Task<IActionResult> SubmitBookingProof([FromBody] SubmitBookingProofDTO dto)
        {
            try
            {
                var proof = await _bookingService.SubmitBookingProofAsync(dto);
                var successResponse = new AppResponse<BookingFinishProof>()
                    .SetSuccessResponse(proof, KeyConst.BookingProof, BookingConst.ProofSubmittedSuccessfully);
                return Ok(successResponse);
            }
            catch (KeyNotFoundException)
            {
                var errorResponse = new AppResponse<BookingFinishProof>()
                    .SetErrorResponse(KeyConst.Booking, BookingConst.BookingNotFound);
                return NotFound(errorResponse);
            }
            catch (InvalidOperationException)
            {
                var errorResponse = new AppResponse<BookingFinishProof>()
                    .SetErrorResponse(KeyConst.Status, BookingConst.InvalidBookingStatusForProof);
                return BadRequest(errorResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new AppResponse<BookingFinishProof>()
                    .SetErrorResponse(KeyConst.Error, $"{BookingConst.ProofSubmissionFailed}: {ex.Message}");
                return StatusCode(500, errorResponse);
            }
        }
        /// <summary>
        /// Retrieves the booking calendar for a housekeeper based on the specified view and navigation.
        /// </summary>
        /// <param name="housekeeperId">The unique identifier of the housekeeper.</param>
        /// <param name="referenceDate">The reference date for the calendar (optional, defaults to today).</param>
        /// <param name="navigationMode">The navigation mode ('next', 'previous', or 'today', defaults to 'today').</param>
        /// <param name="viewMode">The view mode ('month', 'week', or 'day', defaults to 'month').</param>
        /// <param name="cancellationToken">Token to cancel the request (optional).</param>
        /// <returns>A standardized response containing the calendar data or error messages.</returns>
        /// <response code="200">Returns the calendar data with success status.</response>
        /// <response code="400">If the request is invalid (e.g., missing housekeeperId or invalid mode).</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet("BookingListCalendar")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHousekeeperBookings(
            [FromQuery] DateTime? referenceDate = null,
            [FromQuery] string navigationMode = "today",
            [FromQuery] string viewMode = "month"
            )
        {
            var response = new AppResponse<CalendarBookingDTO>();

            try
            {
                

                var result = await _bookingService.GetHousekeeperBookings(
                    User,
                    referenceDate,
                    navigationMode,
                    viewMode
                );

                return Ok(response.SetSuccessResponse(result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(response.SetErrorResponse("validation", [ex.Message]));
            }
            catch (OperationCanceledException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    response.SetErrorResponse("error", ["Request was canceled."]));
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using ILogger)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    response.SetErrorResponse("error", ["An unexpected error occurred."]));
            }
        }
    }
}