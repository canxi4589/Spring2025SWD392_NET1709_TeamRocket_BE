﻿using HCP.Repository.Entities;
using HCP.Service.DTOs.CleaningServiceDTO;
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
    }
}