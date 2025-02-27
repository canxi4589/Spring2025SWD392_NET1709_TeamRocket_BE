using HCP.Service.DTOs.BookingDTO;
using HCP.Service.DTOs.CustomerDTO;
using HCP.Service.Services.BookingService;
using HCP.Service.Services.CustomerService;
using HCP.Service.Services.ListService;
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
        private readonly IBookingService _bookingService;
        private readonly ICustomerService _customerService;
        public BookingController(IBookingService bookingService, ICustomerService customerService)
        {
            _bookingService = bookingService;
            _customerService = customerService;
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
            var bookingDetail = await _bookingService.GetBookingDetailById(id);
            if (bookingDetail != null)
            {
                return Ok(new AppResponse<BookingHistoryDetailResponseDTO>().SetSuccessResponse(bookingDetail));
            }
            return NotFound();
        }
    }
}
