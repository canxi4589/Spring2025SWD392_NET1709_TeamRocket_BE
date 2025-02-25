using HCP.Repository.Entities;
using HCP.Service.DTOs.BookingDTO;

namespace HCP.Service.Services.BookingService
{
    public interface IBookingService
    {
        Task<List<BookingHistoryResponseDTO>> GetBookingByUser(AppUser user);
        Task<BookingHistoryDetailResponseDTO> GetBookingDetailById(Guid id);
    }
}