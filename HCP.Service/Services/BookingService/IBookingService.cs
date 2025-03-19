using HCP.DTOs.DTOs.BookingDTO;
using HCP.DTOs.DTOs.CheckoutDTO;
using HCP.Repository.Entities;
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
        Task<BookingCountDTO> GetBookingCountHousekeeper(ClaimsPrincipal claim);
        Booking UpdateStatusBooking(Guid id, string status);
        Task DeleteBooking(Guid id);
        Task<Payment> CreatePayment(Guid bookingId, decimal amount, string paymentMethod = "VNPay");
        Task<Payment> UpdatePaymentStatusAsync(Guid paymentId, string status);
        Task<BookingListResponseDto> GetHousekeeperBookingsAsync(
          ClaimsPrincipal userClaims,
          int page,
          int pageSize,
          string? status);
        Task<Booking> CreateBookingAsync1(CheckoutResponseDTO1 dto, string uid);
        Task<BookingFinishProof> SubmitBookingProofAsync(SubmitBookingProofDTO dto);
        Task<BookingCancelDTO> cancelBooking(Guid bookingId, AppUser user);
        Task<CalendarBookingDTO> GetHousekeeperBookings(
            ClaimsPrincipal userClaims,
            DateTime? referenceDate = null,
            string navigationMode = "today",
            string viewMode = "month"
        );
    }
}