﻿using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBookingService _bookingService;
        private readonly ICustomerService _customerService;

        public BookingController(IUnitOfWork unitOfWork, IBookingService bookingService, ICustomerService customerService)
        {
            _unitOfWork = unitOfWork;
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
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetHousekeeperBookings([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? status = null)
        {
            var userClaims = User;
            try
            {
                var response = await _bookingService.GetHousekeeperBookingsAsync(userClaims, page, pageSize);
                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}