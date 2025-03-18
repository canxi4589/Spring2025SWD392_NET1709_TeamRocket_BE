using HCP.DTOs.DTOs;
using HCP.DTOs.DTOs.CleaningServiceDTO;
using HCP.DTOs.DTOs.RequestDTO;
using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.Interfaces;
using HCP.Repository.Repository;
using HCP.Service.Services.CleaningService1;
using HCP.Service.Services.EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HCP.Service.Services.RequestService
{
    public class HandleRequestService : IHandleRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ICleaningService1 _cleaningService;
        private readonly IRequestRepository _requestRepository;

        public HandleRequestService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IEmailSenderService emailSenderService, ICleaningService1 cleaningService, IRequestRepository requestRepository)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailSenderService = emailSenderService;
            _cleaningService = cleaningService;
            _requestRepository = requestRepository;
        }

        public async Task<List<PendingRequestDTO>> GetPendingCreateServiceRequestsAsync()
        {
            var pendingRequests = await _requestRepository.GetCleaningServices(ServiceStatus.Pending.ToString());

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
                    .Select(a => new DTOs.DTOs.RequestDTO.AdditionalServiceDTO { Name = a.Name, Amount = a.Amount, Description = a.Description, Duration = a.Duration, Url = a.Url })
                    .ToList();

                item.ServiceImages = pendingRequest.ServiceImages
                    .Select(si => new DTOs.DTOs.RequestDTO.ServiceImgDTO { LinkUrl = si.LinkUrl })
                    .ToList();

                item.ServiceTimeSlots = pendingRequest.ServiceTimeSlots
                    .Select(sts => new DTOs.DTOs.RequestDTO.ServiceTimeSlotDTO { DayOfWeek = sts.DayOfWeek, StartTime = sts.StartTime, EndTime = sts.EndTime })
                    .ToList();

                item.ServiceDistanceRule = pendingRequest.DistancePricingRules
                    .Select(dpr => new DTOs.DTOs.RequestDTO.DistanceRuleDTO
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

        public async Task<PendingRequestDTO> GetPendingCreateServiceDetailAsync(Guid id)
        {
            var pendingRequests = await _unitOfWork.Repository<CleaningService>()
                .GetAll()
                .Where(cs => cs.Id == id)
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
                    CategoryName = categories.TryGetValue(pendingRequest.CategoryId, out var category) && category != null ? category.CategoryName : string.Empty,
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
                    .Select(a => new DTOs.DTOs.RequestDTO.AdditionalServiceDTO { Name = a.Name, Amount = a.Amount, Description = a.Description, Duration = a.Duration, Url = a.Url })
                    .ToList();

                item.ServiceImages = pendingRequest.ServiceImages
                    .Select(si => new DTOs.DTOs.RequestDTO.ServiceImgDTO { LinkUrl = si.LinkUrl })
                    .ToList();

                item.ServiceTimeSlots = pendingRequest.ServiceTimeSlots
                    .Select(sts => new DTOs.DTOs.RequestDTO.ServiceTimeSlotDTO { DayOfWeek = sts.DayOfWeek, StartTime = sts.StartTime, EndTime = sts.EndTime })
                    .ToList();

                item.ServiceDistanceRule = pendingRequest.DistancePricingRules
                    .Select(dpr => new DTOs.DTOs.RequestDTO.DistanceRuleDTO
                    {
                        MinDistance = dpr.MinDistance,
                        MaxDistance = dpr.MaxDistance,
                        BaseFee = dpr.BaseFee,
                        ExtraPerKm = dpr.ExtraPerKm
                    }).ToList();
                pendingRequestsDTO.Add(item);
            }

            return pendingRequestsDTO[0];
        }

        public async Task<(bool Success, string Message)> UpdateServiceStatusAsync(ServiceStatusUpdateDto dto, ClaimsPrincipal userClaims)
        {
            var staffId = userClaims.FindFirst("id")?.Value;
            var service = await _unitOfWork.Repository<CleaningService>().FindAsync(cs => cs.Id == dto.ServiceId);
            var housekeeperEmail = await _userManager.GetEmailAsync(await _userManager.FindByIdAsync(service.UserId));
            var housekeeperName = await _userManager.GetUserNameAsync(await _userManager.FindByIdAsync(service.UserId));
            if (service == null)
            {
                return (false, CleaningServiceConst.ServiceNotFound);
            }

            if (service.Status != ServiceStatus.Pending.ToString())
            {
                return (false, RequestConst.UpdateRequestStatusError);
            }

            service.StaffId = staffId;
            service.Status = dto.IsApprove ? ServiceStatus.Active.ToString() : ServiceStatus.Rejected.ToString();

            if (!dto.IsApprove)
            {
                var rejectedEmailBody = EmailBodyTemplate.GetRejectionEmail(housekeeperName, dto.Reason, dto.ServiceId, service.ServiceName);
                _emailSenderService.SendEmail(housekeeperEmail, RequestConst.RejectEmailSubject, rejectedEmailBody);
            }

            await _unitOfWork.Repository<CleaningService>().SaveChangesAsync();
            return (true, RequestConst.UpdateRequestSuccess);
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

        public async Task<List<ApprovalServiceDTO>> GetApprovedServiceByStaffIdAsync(ClaimsPrincipal user)
        {
            var userId = user.FindFirst("id")?.Value;
            var pendingRequests = await _unitOfWork.Repository<CleaningService>()
                .GetAll()
                .Where(cs => cs.StaffId == userId)
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

            var approvalRequestsDTO = new List<ApprovalServiceDTO>();

            foreach (var pendingRequest in pendingRequests)
            {
                var item = new ApprovalServiceDTO
                {
                    ServiceId = pendingRequest.Id,
                    ServiceName = pendingRequest.ServiceName,
                    CategoryId = pendingRequest.CategoryId,
                    CategoryName = categories.TryGetValue(pendingRequest.CategoryId, out var category) && category != null ? category.CategoryName : string.Empty,
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
                    .Select(a => new DTOs.DTOs.RequestDTO.AdditionalServiceDTO { Name = a.Name, Amount = a.Amount, Description = a.Description, Duration = a.Duration, Url = a.Url })
                    .ToList();

                item.ServiceImages = pendingRequest.ServiceImages
                    .Select(si => new DTOs.DTOs.RequestDTO.ServiceImgDTO { LinkUrl = si.LinkUrl })
                    .ToList();

                item.ServiceTimeSlots = pendingRequest.ServiceTimeSlots
                    .Select(sts => new DTOs.DTOs.RequestDTO.ServiceTimeSlotDTO { DayOfWeek = sts.DayOfWeek, StartTime = sts.StartTime, EndTime = sts.EndTime })
                    .ToList();

                item.ServiceDistanceRule = pendingRequest.DistancePricingRules
                    .Select(dpr => new DTOs.DTOs.RequestDTO.DistanceRuleDTO
                    {
                        MinDistance = dpr.MinDistance,
                        MaxDistance = dpr.MaxDistance,
                        BaseFee = dpr.BaseFee,
                        ExtraPerKm = dpr.ExtraPerKm
                    }).ToList();
                approvalRequestsDTO.Add(item);
            }
            return approvalRequestsDTO;
        }

        //public async Task<ApprovalServiceDTO> GetApprovalServiceDTODetailAsync(Guid id)
        //{
        //    var pendingRequests = await _unitOfWork.Repository<CleaningService>()
        //        .GetAll()
        //        .Where(cs => cs.Id == id)
        //        .Include(cs => cs.AdditionalServices)
        //        .Include(cs => cs.ServiceImages)
        //        .Include(cs => cs.ServiceTimeSlots)
        //        .Include(cs => cs.DistancePricingRules)
        //        .ToListAsync();

        //    var categoryIds = pendingRequests.Select(cs => cs.CategoryId).Distinct().ToList();
        //    var categories = await _unitOfWork.Repository<ServiceCategory>()
        //        .GetAll()
        //        .Where(sc => categoryIds.Contains(sc.Id))
        //        .ToDictionaryAsync(sc => sc.Id);

        //    var approvalRequestsDTO = new List<ApprovalServiceDTO>();

        //    foreach (var pendingRequest in pendingRequests)
        //    {
        //        var item = new ApprovalServiceDTO
        //        {
        //            ServiceId = pendingRequest.Id,
        //            ServiceName = pendingRequest.ServiceName,
        //            CategoryId = pendingRequest.CategoryId,
        //            CategoryName = categories.TryGetValue(pendingRequest.CategoryId, out var category) && category != null ? category.CategoryName : "Unknown Category",
        //            PictureUrl = categories.TryGetValue(pendingRequest.CategoryId, out category) && category != null ? category.PictureUrl : string.Empty,
        //            Description = pendingRequest.Description,
        //            Status = pendingRequest.Status,
        //            Price = pendingRequest.Price,
        //            City = pendingRequest.City,
        //            District = pendingRequest.District,
        //            PlaceId = pendingRequest.PlaceId,
        //            AddressLine = pendingRequest.AddressLine,
        //            Duration = pendingRequest.Duration,
        //            CreatedAt = pendingRequest.CreatedAt,
        //            UpdatedAt = pendingRequest.UpdatedAt,
        //            UserId = pendingRequest.UserId,
        //            UserName = await GetUserNameByIdAsync(pendingRequest.UserId)
        //        };

        //        item.AdditionalServices = pendingRequest.AdditionalServices
        //            .Select(a => new DTOs.RequestDTO.AdditionalServiceDTO { Name = a.Name, Amount = a.Amount })
        //            .ToList();

        //        item.ServiceImages = pendingRequest.ServiceImages
        //            .Select(si => new DTOs.RequestDTO.ServiceImgDTO { LinkUrl = si.LinkUrl })
        //            .ToList();

        //        item.ServiceTimeSlots = pendingRequest.ServiceTimeSlots
        //            .Select(sts => new DTOs.RequestDTO.ServiceTimeSlotDTO { DayOfWeek = sts.DayOfWeek, StartTime = sts.StartTime, EndTime = sts.EndTime })
        //            .ToList();

        //        item.ServiceDistanceRule = pendingRequest.DistancePricingRules
        //            .Select(dpr => new DTOs.RequestDTO.DistanceRuleDTO
        //            {
        //                MinDistance = dpr.MinDistance,
        //                MaxDistance = dpr.MaxDistance,
        //                BaseFee = dpr.BaseFee,
        //                ExtraPerKm = dpr.ExtraPerKm
        //            }).ToList();
        //        approvalRequestsDTO.Add(item);
        //    }

        //    return approvalRequestsDTO[0];
        //}

    }
}
