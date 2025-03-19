using HCP.DTOs.DTOs.AdminManagementDTO;
using HCP.DTOs.DTOs.BookingDTO;
using Microsoft.AspNetCore.Mvc;
using static HCP.DTOs.DTOs.AdminManagementDTO.ChartDataAdminDTO;
using static HCP.DTOs.DTOs.AdminManagementDTO.ServiceAdminDTO;
using static HCP.DTOs.DTOs.AdminManagementDTO.ServiceCategoryAdminDTO;

namespace HCP.Service.Services.AdminManService
{
    public interface IAdminManService
    {
        Task<CreateStaffResponseDTO> CreateStaff(CreateStaffRequestDTO request);
        Task<UserAdminListDTO> GetUsersByAdminCustom(string? search, bool includeStaff, bool includeCustomers, bool includeHousekeepers, int? pageIndex, int? pageSize);
        Task<ServiceCategoryAdminShowListDTO> GetAllServiceCategoriesAsync(string? search, int? pageIndex, int? pageSize, int? day, int? month, int? year);
        Task<ChartCategoryDataAdminShowListDTO> GetCategoryChart(bool dayChart, bool yearChart, bool yearsChart, int? dayStart, int? monthStart, int? yearStart, int? dayEnd, int? monthEnd, int? yearEnd);
        Task<ServiceAdminShowListDTO> GetAllServicesAsync(string? search, int? pageIndex, int? pageSize, int? day, int? month, int? year);
        Task<ChartDataAdminShowListDTO> GetRevenueChartDatas(bool dayChart, bool weekChart, bool yearChart, bool yearsChart, int? dayStart, int? monthStart, int? yearStart, int? dayEnd, int? monthEnd, int? yearEnd);
        Task<BookingHistoryResponseListDTO> GetAllBookingByCateAndServiceAdmin(bool isService, bool isCategory, Guid Id, int? pageIndex, int? pageSize, string? status, int? day, int? month, int? year);
    }
}