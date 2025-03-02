using HCP.Repository.Entities;
using HCP.Service.DTOs.BookingDTO;
using HCP.Service.Services.ListService;
using System.Security.Claims;

namespace HCP.Service.Services.BookingService
{
    public interface IBookingService
    {
        Task<BookingHistoryResponseListDTO> GetBookingByUser(AppUser user, int? pageIndex, int? pageSize);
        Task<BookingHistoryDetailResponseDTO> GetBookingDetailById(Guid id);
        //Task<Guid> CreateBooking(BookingDTO bookingDTO);
        Task<CheckoutResponseDTO> GetCheckoutInfo(CheckoutRequestDTO request, ClaimsPrincipal userClaims);
    }
}