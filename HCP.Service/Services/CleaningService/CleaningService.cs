using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.Services.ListService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient.DataClassification;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.Services.CleaningService1
{
    public class CleaningService1 : ICleaningService1
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;


        public CleaningService1(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<List<CategoryDTO>> GetAllCategories()
        {
            var categories = _unitOfWork.Repository<ServiceCategory>().GetAll();

            return categories.Select(c => new CategoryDTO
            {
                id = c.Id,
                name = c.CategoryName,
                description = c.Description ?? "",
                imgUrl = c.PictureUrl
            }).ToList();
        }
        public async Task<List<CleaningServiceItemDTO>> GetAllServiceItems()
        {
            var list = _unitOfWork.Repository<CleaningService>().GetAll().Include(c => c.Category);
            return list.Select(c => new CleaningServiceItemDTO
            {
                id = c.Id,
                name = c.ServiceName,
                category = c.Category.CategoryName,
                overallRating = c.Rating,
                location = c.AddressLine + ", " + c.District + ", " + c.City,
                price = c.Price,

            }).ToList();
        }
        public async Task<CleaningServiceListDTO> GetAllServiceItems(int? pageIndex, int? pageSize)
        {
            var list = _unitOfWork.Repository<CleaningService>().GetAll().Include(c => c.Category).Include(c => c.ServiceImages);
            var list1 = list.Select(c => new CleaningServiceItemDTO
            {
                id = c.Id,
                name = c.ServiceName,
                category = c.Category.CategoryName,
                overallRating = c.Rating,
                price = c.Price,
                location = c.AddressLine,
                CategoryName = c.Category.CategoryName,
                Url = c.ServiceImages.FirstOrDefault().LinkUrl,
                
            });
            if (pageIndex == null || pageSize == null)
            {
                var temp1 = await PaginatedList<CleaningServiceItemDTO>.CreateAsync(list1, 1, list1.Count());
                return new CleaningServiceListDTO
                {
                    Items = temp1,
                    hasNext = temp1.HasNextPage,
                    hasPrevious = temp1.HasPreviousPage,
                    totalCount = temp1.TotalCount,
                    totalPages = temp1.TotalPages,
                };
            }
            var temp = await PaginatedList<CleaningServiceItemDTO>.CreateAsync(list1, (int)pageIndex, (int)pageSize);
            var temp2 = new CleaningServiceListDTO
            {
                Items = temp,
                hasNext = temp.HasNextPage,
                hasPrevious = temp.HasPreviousPage,
                totalCount = list1.Count(),
                totalPages = temp.TotalPages,
            };
            return temp2;
        }
        public async Task<PaginatedList<CategoryDTO>> GetAllCategories(int pageIndex, int pageSize)
        {
            var categories = _unitOfWork.Repository<ServiceCategory>().GetAll();

            var categoryList = categories.Select(c => new CategoryDTO
            {
                id = c.Id,
                name = c.CategoryName,
                description = c.Description ?? "",
                imgUrl = c.PictureUrl
            });

            return await PaginatedList<CategoryDTO>.CreateAsync(categoryList, pageIndex, pageSize);
        }
        public async Task<ServiceDetailDTO> GetServiceById(Guid serviceId)
        {
            var services = await _unitOfWork.Repository<CleaningService>().ListAsync(
                filter: c => c.Id == serviceId,
                includeProperties: query => query
                    .Include(c => c.Category)
                    .Include(c => c.ServiceRatings)
                    .Include(c => c.ServiceImages)
                    .Include(c => c.ServiceSteps)
                    .Include(c => c.AdditionalServices)
                    .Include(c => c.User)
            );

            var service = services.FirstOrDefault();
            if (service == null)
                return null!;
            var user = await _userManager.FindByIdAsync(service.UserId);
            var address = await _unitOfWork.Repository<Address>().FindAsync(c => c.UserId == user.Id && c.IsDefault) ?? new Address();
            var service1 = await _unitOfWork.Repository<CleaningService>().ListAsync(filter: c => c.UserId == user.Id,
                orderBy: query => query.OrderByDescending(u => u.Id)
);
            return new ServiceDetailDTO
            {
                id = service.Id,
                name = service.ServiceName,
                numOfBooks = service.Bookings?.Count ?? 0,
                location = $"{service.City}, {service.District}",
                reviews = service.ServiceRatings.Any() ? service.ServiceRatings.Average(r => r.Rating) : 0,
                numOfReviews = service.ServiceRatings.Count,
                Price = service.Price,
                numOfPics = service.ServiceImages.Count,
                overview = service.Description,
                images = service.ServiceImages.Select(i => new ImgDTO { url = i.LinkUrl }).ToList(),
                steps = service.ServiceSteps.Select(s => s.StepDescription).ToList(),
                additionalServices = service.AdditionalServices.Select(a => new AdditionalServicedDTO
                {
                    id = a.Id,
                    name = a.Name,
                    price = a.Amount.ToString("0.0"),
                    url = a.Url,
                    Duration = a.Duration,
                    Description = a.Description
                }).ToList(),
                housekeeper =
                     new housekeeperDetailDTO
                     {
                         id = service.User.Id,
                         name = service.User.FullName,
                         //review = service.User.ServiceRatings.Any() ?
                         //         service.User.ServiceRatings.Average(r => r.Rating).ToString("0.0") : "No reviews",
                         review = "No Reviews",
                         avatar = service.User.Avatar,
                         memberSince = /*service.User..ToString("yyyy-MM-dd")*/"2025-03-12",
                         address = address.City != null && address.District != null ? $"{address.City}, {address.District},{address.AddressLine1}" : string.Empty,
                         email = service.User.Email,
                         mobile = service.User.PhoneNumber,
                         numOfServices = service1.Count()
                     }
            };
        }
        public async Task<List<ServiceTimeSlotDTO1>> GetAllServiceTimeSlot(Guid serviceId, DateTime targetDate, string dayOfWeek)
        {
            var slots = await _unitOfWork.Repository<ServiceTimeSlot>().ListAsync(
                filter: s => s.ServiceId == serviceId && s.DayOfWeek == dayOfWeek,
                orderBy: s => s.OrderBy(c => c.StartTime)
            );

            var bookedSlots = await _unitOfWork.Repository<Booking>().ListAsync(
                filter: b => b.CleaningServiceId == serviceId && b.PreferDateStart == targetDate,
                orderBy: b => b.OrderBy(c => c.TimeStart) 
            );

            var availableSlots = new List<ServiceTimeSlotDTO1>();

            foreach (var slot in slots)
            {
                bool isAvailable = !bookedSlots.Any(b =>
                    b.TimeStart < slot.EndTime && b.TimeEnd > slot.StartTime 
                );

                if (isAvailable)
                {
                    availableSlots.Add(new ServiceTimeSlotDTO1
                    {
                        Id = slot.Id,
                        TimeStart = slot.StartTime,
                        TimeEnd = slot.EndTime,
                        DayOfWeek = slot.DayOfWeek
                    });
                }
            }

            return availableSlots;
        }

        public async Task<List<ServiceDetailWithStatusDTO>> GetServiceByUser(ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirst("id")?.Value;
            var services = await _unitOfWork.Repository<CleaningService>().ListAsync(
                filter: c => c.UserId == userId,
                includeProperties: query => query
                    .Include(c => c.Category)
                    .Include(c => c.ServiceRatings)
                    .Include(c => c.ServiceImages)
                    .Include(c => c.ServiceSteps)
                    .Include(c => c.AdditionalServices)
                    .Include(c => c.User)
            );

            if (services == null || !services.Any())
                return null;
            var address = await _unitOfWork.Repository<Address>().FindAsync(c => c.UserId == userId && c.IsDefault) ?? new Address();

            return services.Select(service => new ServiceDetailWithStatusDTO
            {
                id = service.Id,
                name = service.ServiceName,
                status = service.Status,
                numOfBooks = service.Bookings?.Count ?? 0,
                location = $"{service.City}, {service.District}",
                reviews = service.ServiceRatings.Any() ? service.ServiceRatings.Average(r => r.Rating) : 0,
                numOfReviews = service.ServiceRatings.Count,
                numOfPics = service.ServiceImages.Count,
                overview = service.Description,
                images = service.ServiceImages.Select(i => new ImgDTO { url = i.LinkUrl }).ToList(),
                steps = service.ServiceSteps.Select(s => s.StepDescription).ToList(),
                additionalServices = service.AdditionalServices.Select(a => new AdditionalServicedDTO
                {
                    id = a.Id,
                    name = a.Name,
                    price = a.Amount.ToString("0.0"),
                    Description = a.Description,
                    Duration = a.Duration,
                    url = a.Url
                }).ToList(),
                housekeeper = service.User != null
                    ? new housekeeperDetailDTO
                    {
                        id = service.User.Id,
                        name = service.User.FullName,
                        review = "No Reviews",
                        avatar = service.User.Avatar,
                        memberSince = "2025-03-12",
                        address = address.City != null && address.District != null ? $"{address.City}, {address.District},{address.AddressLine1}" : string.Empty,
                        email = service.User.Email,
                        mobile = service.User.PhoneNumber,
                        numOfServices = service.User.Services?.Count ?? 0
                    }
                    : null
            }).ToList();
        }

        public async Task<CreateCleaningServiceDTO?> CreateCleaningServiceAsync(CreateCleaningServiceDTO dto, ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }
            var user = await _userManager.FindByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");

            var newService = new CleaningService
            {
                ServiceName = dto.ServiceName,
                CategoryId = dto.CategoryId,
                Description = dto.Description,
                Status = ServiceStatus.Pending.ToString(),
                Rating = 0,
                RatingCount = 0,
                Price = dto.Price,
                City = dto.City,
                District = dto.District,
                PlaceId = dto.PlaceId,
                AddressLine = dto.AddressLine,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Duration = dto.Duration,
                UserId = userId,
                User = await _userManager.FindByIdAsync(userId),
                Category = await _unitOfWork.Repository<ServiceCategory>().FindAsync(c => c.Id == dto.CategoryId)
            };

            await _unitOfWork.Repository<CleaningService>().AddAsync(newService);
            await _unitOfWork.Repository<CleaningService>().SaveChangesAsync();

            if (dto.AdditionalServices != null)
            {
                var additionalServices = dto.AdditionalServices.Select(a => new AdditionalService
                {
                    Name = a.Name,
                    CleaningServiceId = newService.Id,
                    Amount = a.Amount,
                    IsActive = true,
                    Description = a.Description,
                    Url = a.Description,
                    Duration = a.Duration,
                });

                await _unitOfWork.Repository<AdditionalService>().AddRangeAsync(additionalServices);
            }

            if (dto.ServiceImages != null)
            {
                var images = dto.ServiceImages.Select(img => new ServiceImg
                {
                    CleaningServiceId = newService.Id,
                    LinkUrl = img.LinkUrl
                });

                await _unitOfWork.Repository<ServiceImg>().AddRangeAsync(images);
            }

            if (dto.ServiceSteps != null)
            {
                var steps = dto.ServiceSteps.Select(step => new ServiceSteps
                {
                    ServiceId = newService.Id,
                    StepOrder = step.StepOrder,
                    StepDescription = step.StepDescription
                });

                await _unitOfWork.Repository<ServiceSteps>().AddRangeAsync(steps);
            }

            if (dto.ServiceTimeSlots != null)
            {
                var timeSlots = new List<ServiceTimeSlot>();

                foreach (var timeSlotDTO in dto.ServiceTimeSlots)
                {
                    timeSlots.Add(new ServiceTimeSlot
                    {
                        ServiceId = newService.Id,
                        StartTime = timeSlotDTO.StartTime,
                        EndTime = timeSlotDTO.StartTime.Add(TimeSpan.FromHours(newService.Duration)),
                        DayOfWeek = timeSlotDTO.DayOfWeek,
                        Status = ServiceStatus.Active.ToString()
                    });
                }

                await _unitOfWork.Repository<ServiceTimeSlot>().AddRangeAsync(timeSlots);
            }

            if (dto.ServiceDistanceRule != null)
            {
                var distanceRules = dto.ServiceDistanceRule.Select(rule => new DistancePricingRule
                {
                    CleaningServiceId = newService.Id,
                    MinDistance = rule.MinDistance,
                    MaxDistance = rule.MaxDistance,
                    BaseFee = rule.BaseFee,
                    ExtraPerKm = rule.ExtraPerKm,
                    IsActive = true
                });
                await _unitOfWork.Repository<DistancePricingRule>().AddRangeAsync(distanceRules);
            }
            await _unitOfWork.Repository<CleaningService>().SaveChangesAsync();
            return dto;
        }

        public async Task<CreateCleaningServiceDTO?> UpdateCleaningServiceAsync(Guid serviceId, CreateCleaningServiceDTO dto, ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("");
            }

            var service = await _unitOfWork.Repository<CleaningService>().FindAsync(s => s.Id == serviceId && s.UserId == userId);
            if (service == null)
            {
                throw new KeyNotFoundException("");
            }

            service.ServiceName = dto.ServiceName;
            service.CategoryId = dto.CategoryId;
            service.Description = dto.Description;
            service.Price = dto.Price;
            service.City = dto.City;
            service.District = dto.District;
            service.PlaceId = dto.PlaceId;
            service.AddressLine = dto.AddressLine;
            service.Duration = dto.Duration;
            service.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<CleaningService>().Update(service);

            var existingAdditionalServices = await _unitOfWork.Repository<AdditionalService>().FindAllAsync(a => a.CleaningServiceId == serviceId);
            await _unitOfWork.Repository<AdditionalService>().RemoveRangeAsync(existingAdditionalServices);
            if (dto.AdditionalServices != null)
            {
                var newAdditionalServices = dto.AdditionalServices.Select(a => new AdditionalService
                {
                    Name = a.Name,
                    CleaningServiceId = serviceId,
                    Amount = a.Amount,
                    IsActive = true,
                    Description = a.Description,
                    Url = a.Url,
                    Duration = a.Duration,
                });
                await _unitOfWork.Repository<AdditionalService>().AddRangeAsync(newAdditionalServices);
            }

            var existingImages = await _unitOfWork.Repository<ServiceImg>().FindAllAsync(i => i.CleaningServiceId == serviceId);
            await _unitOfWork.Repository<ServiceImg>().RemoveRangeAsync(existingImages);
            if (dto.ServiceImages != null)
            {
                var newImages = dto.ServiceImages.Select(img => new ServiceImg
                {
                    CleaningServiceId = serviceId,
                    LinkUrl = img.LinkUrl
                });
                await _unitOfWork.Repository<ServiceImg>().AddRangeAsync(newImages);
            }

            var existingSteps = await _unitOfWork.Repository<ServiceSteps>().FindAllAsync(s => s.ServiceId == serviceId);
            await _unitOfWork.Repository<ServiceSteps>().RemoveRangeAsync(existingSteps);
            if (dto.ServiceSteps != null)
            {
                var newSteps = dto.ServiceSteps.Select(step => new ServiceSteps
                {
                    ServiceId = serviceId,
                    StepOrder = step.StepOrder,
                    StepDescription = step.StepDescription
                });
                await _unitOfWork.Repository<ServiceSteps>().AddRangeAsync(newSteps);
            }

            var existingTimeSlots = await _unitOfWork.Repository<ServiceTimeSlot>().FindAllAsync(t => t.ServiceId == serviceId);
            await _unitOfWork.Repository<ServiceTimeSlot>().RemoveRangeAsync(existingTimeSlots);
            if (dto.ServiceTimeSlots != null)
            {
                var newTimeSlots = dto.ServiceTimeSlots.Select(timeSlotDTO => new ServiceTimeSlot
                {
                    ServiceId = serviceId,
                    StartTime = timeSlotDTO.StartTime,
                    EndTime = timeSlotDTO.StartTime.Add(TimeSpan.FromHours(service.Duration)),
                    DayOfWeek = timeSlotDTO.DayOfWeek,
                    Status = ServiceStatus.Active.ToString()
                });
                await _unitOfWork.Repository<ServiceTimeSlot>().AddRangeAsync(newTimeSlots);
            }

            var existingDistanceRules = await _unitOfWork.Repository<DistancePricingRule>().FindAllAsync(d => d.CleaningServiceId == serviceId);
            await _unitOfWork.Repository<DistancePricingRule>().RemoveRangeAsync(existingDistanceRules);
            if (dto.ServiceDistanceRule != null)
            {
                var newDistanceRules = dto.ServiceDistanceRule.Select(rule => new DistancePricingRule
                {
                    CleaningServiceId = serviceId,
                    MinDistance = rule.MinDistance,
                    MaxDistance = rule.MaxDistance,
                    BaseFee = rule.BaseFee,
                    ExtraPerKm = rule.ExtraPerKm,
                    IsActive = true
                });
                await _unitOfWork.Repository<DistancePricingRule>().AddRangeAsync(newDistanceRules);
            }

            await _unitOfWork.Repository<CleaningService>().SaveChangesAsync();
            return dto;
        }

        public async Task<bool> IsTimeSlotAvailable(Guid serviceId, DateTime targetDate, TimeSpan startTime, TimeSpan endTime)
        {
            var isBooked = await _unitOfWork.Repository<Booking>().ExistsAsync(
                b => b.CleaningServiceId == serviceId &&
                     b.PreferDateStart == targetDate &&
                     ((b.TimeStart <= endTime && b.TimeEnd >= startTime) && b.Status == BookingStatus.OnGoing.ToString())
            );

            return !isBooked;
        }

        public async Task<List<AdditionalServicedDTO>> GetAllAdditonalServicesById(Guid serviceId)
        {
            var list = await _unitOfWork.Repository<AdditionalService>().ListAsync(
                filter: c => c.CleaningServiceId == serviceId && c.IsActive,
                orderBy: c => c.OrderBy(c => c.Id)
            );

            var result = list.Select(c => new AdditionalServicedDTO
            {
                id = c.Id,
                name = c.Name,
                price = c.Amount.ToString("F2"),
                url = c.Url,
                Duration = c.Duration,
                Description = c.Description
            }).ToList();

            return result;
        }
    }
}