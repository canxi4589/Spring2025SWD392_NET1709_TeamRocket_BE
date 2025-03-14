﻿using HCP.Repository.Entities;
using HCP.Service.DTOs.BookingDTO;
using HCP.Service.DTOs.CheckoutDTO;
using HCP.Service.Services.ListService;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HCP.Service.Services.BookingService
{
    public interface IBookingService
    {
        Task<BookingHistoryResponseListDTO> GetBookingByUser(AppUser user, int? pageIndex, int? pageSize, string? status, int? day, int? month, int? year);
        Task<BookingHistoryDetailResponseDTO> GetBookingDetailById(Booking booking);
        //Task<Guid> CreateBooking(BookingDTO bookingDTO);
        Task<CheckoutResponseDTO> GetCheckoutInfo(CheckoutRequestDTO request, ClaimsPrincipal userClaims);
        Task<Booking> CreateBookingAsync(CheckoutResponseDTO1 dto, ClaimsPrincipal userClaims);
        Task<Booking> GetBookingById(Guid id);
        Booking UpdateStatusBooking(Guid id, string status);
        Task DeleteBooking(Guid id);
        Task<Payment> CreatePayment(Guid bookingId, decimal amount, string paymentMethod = "VNPay");
        Task<Payment> UpdatePaymentStatusAsync(Guid paymentId, string status);
        Task<BookingListResponseDto> GetHousekeeperBookingsAsync(ClaimsPrincipal userClaims, int page, int pageSize, string? Status);
        Task<Booking> CreateBookingAsync1(CheckoutResponseDTO1 dto, string uid);
    }
}