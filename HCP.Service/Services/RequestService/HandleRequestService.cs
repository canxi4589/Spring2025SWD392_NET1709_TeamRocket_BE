using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.DTOs.RequestDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.Services.RequestService
{
    public class HandleRequestService : IHandleRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public HandleRequestService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<List<PendingRequestDTO>> GetPendingCreateServiceRequestsAsync()
        {
            var pendingRequests = await _unitOfWork.Repository<CleaningService>()
                .GetAll()
                .Where(cs => cs.Status == ServiceStatus.Pending.ToString())
                .Include(cs => cs.AdditionalServices) 
                .Include(cs => cs.ServiceImages)
                .Include(cs => cs.ServiceTimeSlots)
                .Include(cs => cs.DistancePricingRules)
                .ToListAsync(); 

            var categoryIds = pendingRequests.Select(cs => cs.CategoryId).Distinct().ToList();
            var categories = await _unitOfWork.Repository<ServiceCategory>()
                .GetAll()
                .Where(sc => categoryIds.Contains(sc.Id))
                .ToDictionaryAsync(sc => sc.Id); 

            var pendingRequestsDTO = new List<PendingRequestDTO>();

            foreach (var pendingRequest in pendingRequests)
            {
                var item = new PendingRequestDTO
                {
                    ServiceId = pendingRequest.Id,
                    ServiceName = pendingRequest.ServiceName,
                    CategoryId = pendingRequest.CategoryId,
                    CategoryName = categories.TryGetValue(pendingRequest.CategoryId, out var category) && category != null ? category.CategoryName : "Unknown Category",
                    PictureUrl = categories.TryGetValue(pendingRequest.CategoryId, out category) && category != null ? category.PictureUrl : string.Empty,
                    Description = pendingRequest.Description,
                    Status = pendingRequest.Status,
                    Price = pendingRequest.Price,
                    City = pendingRequest.City,
                    District = pendingRequest.District,
                    PlaceId = pendingRequest.PlaceId,
                    AddressLine = pendingRequest.AddressLine,
                    Duration = pendingRequest.Duration,
                    CreatedAt = pendingRequest.CreatedAt,
                    UpdatedAt = pendingRequest.UpdatedAt,
                    UserId = pendingRequest.UserId,
                    UserName = await GetUserNameByIdAsync(pendingRequest.UserId)
                };

                item.AdditionalServices = pendingRequest.AdditionalServices
                    .Select(a => new DTOs.RequestDTO.AdditionalServiceDTO { Name = a.Name, Amount = a.Amount })
                    .ToList();

                item.ServiceImages = pendingRequest.ServiceImages
                    .Select(si => new DTOs.RequestDTO.ServiceImgDTO { LinkUrl = si.LinkUrl })
                    .ToList();

                item.ServiceTimeSlots = pendingRequest.ServiceTimeSlots
                    .Select(sts => new DTOs.RequestDTO.ServiceTimeSlotDTO { DayOfWeek = sts.DayOfWeek, StartTime = sts.StartTime, EndTime = sts.EndTime })
                    .ToList();

                item.ServiceDistanceRule = pendingRequest.DistancePricingRules
                    .Select(dpr => new DTOs.RequestDTO.DistanceRuleDTO
                    {
                        MinDistance = dpr.MinDistance,
                        MaxDistance = dpr.MaxDistance,
                        BaseFee = dpr.BaseFee,
                        ExtraPerKm = dpr.ExtraPerKm
                    }).ToList();
                pendingRequestsDTO.Add(item);
            }
            return pendingRequestsDTO;
        }

        public async Task<(bool Success, string Message)> UpdateServiceStatusAsync(ServiceStatusUpdateDto dto)
        {
            var service = await _unitOfWork.Repository<CleaningService>().FindAsync(cs => cs.Id == dto.ServiceId);
            if (service == null)
            {
                return (false, "Service not found");
            }

            if (service.Status != "Pending")
            {
                return (false, "Service status can only be updated if it is 'Pending'");
            }

            service.Status = dto.IsApprove ? "Active" : "Rejected";

            await _unitOfWork.Repository<CleaningService>().SaveChangesAsync();
            return (true, "Service status updated successfully");
        }

        //public async Task<CreateCleaningServiceDTO> GetAllPendingService()
        //{
        //    var services = _unitOfWork.Repository<CleaningService>().GetAll().Where(cs => cs.Status == "Pending");
        //    var pendingServices = new List<CreateCleaningServiceDTO>();
        //    foreach (var service in services)
        //    {
        //        var user = await _userManager.FindByIdAsync(service.UserId);
        //        var serviceDetail = new CreateCleaningServiceDTO
        //        {
        //            AdditionalServices = service.AdditionalServices,

        //        };
        //        pendingServices.Services.Add(serviceDetail);
        //    }
        //    return pendingServices;
        //}

        private async Task<string> GetUserNameByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return "Unknown";

            var user = await _userManager.FindByIdAsync(userId);
            return user?.UserName ?? "Unknown";
        }
    }
}
