using HCP.Service.DTOs.BookingDTO;
using HCP.Service.DTOs.CustomerDTO;
using HCP.Service.Services.BookingService;
using HCP.Service.Services.CustomerService;
using HomeCleaningService.Helpers;
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
        public async Task<IActionResult> GetAllBookingByUser()
        {
            var user = await _customerService.GetCustomerByIdAsync(User);
            if (user != null)
            {
                var list = await _bookingService.GetBookingByUser(user);
                return Ok(new AppResponse<List<BookingHistoryResponseDTO>>().SetSuccessResponse(list));
            }
            return NotFound();
        }
        [HttpGet("bookingDetail")]
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
