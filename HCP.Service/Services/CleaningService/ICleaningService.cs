using HCP.DTOs.DTOs.CleaningServiceDTO;
using HCP.DTOs.DTOs.FilterDTO;
using HCP.DTOs.DTOs.HousekeeperDTOs;
using HCP.Repository.Entities;
using HCP.Service.Services.ListService;
using System.Security.Claims;

namespace HCP.Service.Services.CleaningService1
{
    public interface ICleaningService1
    {
        Task<List<CategoryDTO>> GetAllCategories();
        Task<List<CleaningServiceItemDTO>> GetAllServiceItems();
        Task<PaginatedList<CategoryDTO>> GetAllCategories(int pageIndex, int pageSize);
        Task<CleaningServiceListDTO> GetAllServiceItems(int? pageIndex, int? pageSize);
        Task<ServiceDetailDTO> GetServiceById(Guid serviceId);
        Task<List<ServiceTimeSlotDTO1>> GetAllServiceTimeSlot(Guid serviceId, DateTime targetDate, string dayOfWeek);
        Task<bool> IsTimeSlotAvailable(Guid serviceId, DateTime targetDate, TimeSpan startTime, TimeSpan endTime);
        Task<List<ServiceDetailWithStatusDTO>> GetServiceByUser(ClaimsPrincipal userClaims);
        Task<CreateCleaningServiceDTO?> CreateCleaningServiceAsync(CreateCleaningServiceDTO dto, ClaimsPrincipal userClaims);
        Task<List<AdditionalServicedDTO>> GetAllAdditonalServicesById(Guid serviceId);
        Task<CreateCleaningServiceDTO?> UpdateCleaningServiceAsync(Guid serviceId, CreateCleaningServiceDTO dto, ClaimsPrincipal userClaims);
        Task<HousekeeperServiceDetailDTO?> GethousekeeperCleaningServiceDetailAsync(Guid serviceId, ClaimsPrincipal userClaims);
        Task<ServiceOverviewListDTO> GetServiceByUserFilter(string? status, ClaimsPrincipal userClaims, int? pageIndex, int? pageSize);
        Task<CleaningServiceListDTO> GetAllServiceItems(
            string? userPlaceId,
            double? maxDistanceKm,
            int? pageIndex,
            int? pageSize,
            List<Guid>? categoryIds = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            List<decimal>? ratings = null,
            string? search = null);
        Task<ServiceFilterOptionsDTO> GetFilterOptionsAsync();
        Task<CleaningServiceTopListDTO> GetTopServiceItems(ClaimsPrincipal claims, bool dayTop, bool monthTop, bool yearTop,
    int? pageIndex, int? pageSize, int? dayStart, int? monthStart, int? yearStart, int? dayEnd, int? monthEnd, int? yearEnd,
    string? search, int? tops = 3);
        Task<List<HousekeeperSkillDTO>> GetHousekeeperCategories(ClaimsPrincipal housekeeper);
        Task<List<ServiceDetailWithStatusDTO1>> GetServiceByUser1(ClaimsPrincipal userClaims);
    }
}