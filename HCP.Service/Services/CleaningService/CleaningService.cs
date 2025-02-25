using HCP.Repository.Entities;
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
                description = "hehe",
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
                location = c.AddressLine + ", " + c.Province+", " + c.City,
                price = c.Price,
                
            }).ToList();
        }
        public async Task<CleaningServiceListDTO> GetAllServiceItems(int? pageIndex, int? pageSize)
        {
            var list = _unitOfWork.Repository<CleaningService>().GetAll().Include(c => c.Category);
            var list1 = list.Select(c => new CleaningServiceItemDTO
            {
                id = c.Id,
                name = c.ServiceName,
                category = c.Category.CategoryName,
                overallRating = c.Rating,
                price = c.Price,
                location = c.AddressLine + ", " + c.Province + ", " + c.City
            });
            if(pageIndex == null || pageSize == null)
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
            var temp =await PaginatedList<CleaningServiceItemDTO>.CreateAsync(list1, (int)pageIndex, (int)pageSize);
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
                description = "hehe",
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
                return null;

            return new ServiceDetailDTO
            {
                id = service.Id,
                name = service.ServiceName,
                numOfBooks = service.Bookings?.Count ?? 0,
                location = $"{service.City}, {service.Province}",
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
                }).ToList(),
                housekeeper = service.User != null
                    ? new housekeeperDetailDTO
                    {
                        id = service.User.Id,
                        name = service.User.FullName,
                        //review = service.User.ServiceRatings.Any() ?
                        //         service.User.ServiceRatings.Average(r => r.Rating).ToString("0.0") : "No reviews",
                        review = "No Reviews",
                        avatar = service.User.Avatar,
                        memberSince = /*service.User..ToString("yyyy-MM-dd")*/"2025-03-12",
                        address = /*$"{service.User.City}, {service.User.Province}"*/ "HCM,Phu Giao",
                        email = service.User.Email,
                        mobile = service.User.PhoneNumber,
                        numOfServices = service.User.Services?.Count ?? 0
                    }
                    : null
            };
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

            return services.Select(service => new ServiceDetailWithStatusDTO
            {
                id = service.Id,
                name = service.ServiceName,
                status = service.Status,
                numOfBooks = service.Bookings?.Count ?? 0,
                location = $"{service.City}, {service.Province}",
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
                }).ToList(),
                housekeeper = service.User != null
                    ? new housekeeperDetailDTO
                    {
                        id = service.User.Id,
                        name = service.User.FullName,
                        review = "No Reviews",
                        avatar = service.User.Avatar,
                        memberSince = "2025-03-12",
                        address = "HCM,Phu Giao",
                        email = service.User.Email,
                        mobile = service.User.PhoneNumber,
                        numOfServices = service.User.Services?.Count ?? 0
                    }
                    : null
            }).ToList();
        }

        public async Task<CleaningService?> CreateCleaningServiceAsync(CreateCleaningServiceDTO dto, ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirst("id")?.Value;
            if (userId == null) return null;

            var newService = new CleaningService
            {
                ServiceName = dto.ServiceName,
                CategoryId = dto.CategoryId,
                Description = dto.Description,
                Status = "Pending",
                Rating = 0,
                RatingCount = 0,
                Price = dto.Price,
                City = dto.City,
                Province = dto.Province,
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
                    IsActive = true
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
                        DateStart = timeSlotDTO.DateStart,
                        DayOfWeek = timeSlotDTO.DayOfWeek,
                        IsBook = false,
                        Status = "Active"
                    });
                }

                await _unitOfWork.Repository<ServiceTimeSlot>().AddRangeAsync(timeSlots);
            }

            await _unitOfWork.Repository<CleaningService>().SaveChangesAsync();
            return newService;
        }
    }
}
