using HCP.DTOs.DTOs.BookingDTO;
using HCP.Repository.Entities;

namespace HCP.Service.Services.BookingService
{
    public interface IBookingTransactionService
    {
        Task<BookingTransactionShowListDTO> GetBookingTransactionHistory(AppUser user, bool customerOrNot, int? pageIndex, int? pageSize, int? day, int? month, int? year, string? status);
        Task<BookingTransactionDetailShowDTO> GetBookingTransactionHistoryDetail(Payment payment);
    }
}