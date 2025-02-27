using HCP.Repository.Entities;
using HCP.Service.DTOs.BookingDTO;
using HCP.Service.Services.ListService;

namespace HCP.Service.Services.BookingService
{
    public interface IBookingService
    {
        Task<BookingHistoryResponseListDTO> GetBookingByUser(AppUser user, int? pageIndex, int? pageSize, string? status, int? day, int? month, int? year);
        Task<BookingHistoryDetailResponseDTO> GetBookingDetailById(Guid id);
    }
}