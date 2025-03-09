using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.BookingDTO;
using HCP.Service.DTOs.WalletDTO;
using HCP.Service.Services.BookingService;
using HCP.Service.Services.CustomerService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingTransactionController : ControllerBase
    {
        private readonly IBookingTransactionService _bookingTransactionService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICustomerService _customerService;
        private readonly IUnitOfWork _unitOfWork;

        public BookingTransactionController(IBookingTransactionService bookingTransactionService, UserManager<AppUser> userManager, ICustomerService customerService, IUnitOfWork unitOfWork)
        {
            _bookingTransactionService = bookingTransactionService;
            _userManager = userManager;
            _customerService = customerService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet()]
        [Authorize]
        public async Task<IActionResult> GetBookingTransactionHistoryPaging(bool customerOrNot, int? pageIndex, int? pageSize, int? day, int? month, int? year, string? status)
        {
            var user = await _customerService.GetCustomerAsync(User);
            if (user != null)
            {
                var list = await _bookingTransactionService.GetBookingTransactionHistory(user, customerOrNot, pageIndex, pageSize, day, year, month, status);
                return Ok(new AppResponse<BookingTransactionShowListDTO>().SetSuccessResponse(list));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse("Error", "User not found"));
        }
        [HttpGet("detail")]
        [Authorize]
        public async Task<IActionResult> GetBookingTransactionHistoryDetail(Guid paymentId)
        {
            var payment = await _unitOfWork.Repository<Payment>().FindAsync(c => c.Id == paymentId);
            var user = await _customerService.GetCustomerAsync(User);
            if (payment != null)
            {
                if (user != null)
                {
                    var list = await _bookingTransactionService.GetBookingTransactionHistoryDetail(payment);
                    return Ok(new AppResponse<BookingTransactionDetailShowDTO>().SetSuccessResponse(list));
                }
                return NotFound(new AppResponse<string>().SetErrorResponse("Error", "User not found"));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse("Error", "Transaction of booking not found"));
        }
    }
}
