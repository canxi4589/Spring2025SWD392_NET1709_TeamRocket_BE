using HCP.DTOs.DTOs.CleaningServiceDTO;
using HCP.DTOs.DTOs.FilterDTO;
using HCP.DTOs.DTOs.HousekeeperDTOs;
using HCP.DTOs.DTOs.RequestDTO;
using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.Interfaces;
using HCP.Service.Services.ListService;
using HCP.Service.Services.RatingService;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HCP.Service.Services.CleaningService1
{
    public class CleaningService1 : ICleaningService1
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IRatingService ratingService;
        private readonly IGoongDistanceService goongDistanceService;

        public CleaningService1(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IRatingService ratingService, IGoongDistanceService goongDistanceService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            this.ratingService = ratingService;
            this.goongDistanceService = goongDistanceService;
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
            var list = _unitOfWork.Repository<CleaningService>().GetAll().Where(c => c.Status == ServiceStatus.Active.ToString()).Include(c => c.Category).Include(c => c.ServiceImages);
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
        public async Task<CleaningServiceTopListDTO> GetTopServiceItems(ClaimsPrincipal claims, bool dayTop, bool monthTop, bool yearTop,
    int? pageIndex, int? pageSize, int? dayStart, int? monthStart, int? yearStart, int? dayEnd, int? monthEnd, int? yearEnd,
    string? search, int? tops = 3)
        {
            var userId = claims.FindFirst("id")?.Value;
            var serviceRepo = _unitOfWork.Repository<CleaningService>();

            // Define date range for filtering
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (dayTop && dayStart.HasValue && monthStart.HasValue && yearStart.HasValue &&
                dayEnd.HasValue && monthEnd.HasValue && yearEnd.HasValue)
            {
                startDate = new DateTime(yearStart.Value, monthStart.Value, dayStart.Value);
                endDate = new DateTime(yearEnd.Value, monthEnd.Value, dayEnd.Value);
            }
            else if (monthTop && yearStart.HasValue && monthStart.HasValue)
            {
                startDate = new DateTime(yearStart.Value, monthStart.Value, 1);
                endDate = startDate.Value.AddMonths(1).AddDays(-1);
            }
            else if (yearTop && yearStart.HasValue)
            {
                startDate = new DateTime(yearStart.Value, 1, 1);
                endDate = new DateTime(yearStart.Value, 12, 31);
            }

            // Query services with related data
            var servicesQuery = await serviceRepo.ListAsync(
                filter: s =>
                    s.Status == ServiceStatus.Active.ToString() &&
                    (string.IsNullOrEmpty(search) || s.ServiceName.Contains(search)) &&
                    s.UserId.Equals(userId),
                includeProperties: q => q.Include(s => s.Category)
                                       .Include(s => s.ServiceImages)
                                       .Include(s => s.ServiceRatings)
                                       .Include(s => s.Bookings)
                                       .ThenInclude(s => s.Payments)
            );

            // Convert to DTO with revenue calculation
            var serviceDTOs = servicesQuery.Select(c => new CleaningServiceTopItemsDTO
            {
                id = c.Id,
                name = c.ServiceName,
                category = c.Category.CategoryName,
                overallRating = c.Rating,
                price = c.Price,
                location = c.AddressLine,
                CategoryName = c.Category.CategoryName,
                Url = c.ServiceImages.FirstOrDefault()?.LinkUrl,
                revenue = c.Bookings
                    .Where(b => b.Payments != null && b.Payments.Any(p => p.Status == "succeed"))
                    .SelectMany(b => b.Payments)
                    .Where(p => p.Status == "succeed" &&
                               (startDate == null || p.PaymentDate >= startDate) &&
                               (endDate == null || p.PaymentDate <= endDate))
                    .Sum(p => p.Amount) * 0.9m,
                NumberOfBooking = c.Bookings.Count()
            })
            .OrderByDescending(dto => dto.revenue)
            .Take(tops ?? 3);
            if (pageIndex == null || pageSize == null)
            {
                var temp1 = PaginatedList<CleaningServiceTopItemsDTO>.CreateAsync(serviceDTOs.ToList(), 1, serviceDTOs.Count());
                return new CleaningServiceTopListDTO
                {
                    Items = temp1,
                    hasNext = temp1.HasNextPage,
                    hasPrevious = temp1.HasPreviousPage,
                    totalCount = temp1.TotalCount,
                    totalPages = temp1.TotalPages,
                };
            }
            var temp2 = PaginatedList<CleaningServiceTopItemsDTO>.CreateAsync(serviceDTOs.ToList(), (int)pageIndex, (int)pageSize);
            return new CleaningServiceTopListDTO
            {
                Items = temp2,
                hasNext = temp2.HasNextPage,
                hasPrevious = temp2.HasPreviousPage,
                totalCount = serviceDTOs.Count(),
                totalPages = temp2.TotalPages,
            };
        }
        public async Task<CleaningServiceListDTO> GetAllServiceItems(
            string? userPlaceId,
            double? maxDistanceKm,
            int? pageIndex,
            int? pageSize,
            List<Guid>? categoryIds = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            List<decimal>? ratings = null,
            string? search = null)
        {
            var serviceRepo = _unitOfWork.Repository<CleaningService>();

            var servicesQuery = await serviceRepo.ListAsync(
            filter: s =>
                s.Status == ServiceStatus.Active.ToString() &&
                (string.IsNullOrEmpty(search) || s.ServiceName.Contains(search)) &&
                (categoryIds == null || !categoryIds.Any() || categoryIds.Contains(s.CategoryId)) &&
                (!minPrice.HasValue || s.Price >= minPrice.Value) &&
                (!maxPrice.HasValue || s.Price <= maxPrice.Value) &&
                (ratings == null || !ratings.Any() ||
                 ratings.Any(r => (r == 5 && s.Rating == 5) || (s.Rating >= r && s.Rating < r + 1))),
                includeProperties: q => q.Include(s => s.Category).Include(s => s.ServiceImages).Include(s => s.ServiceRatings)
            );

            var filteredServices = servicesQuery.ToList();

            if (!string.IsNullOrEmpty(userPlaceId))
            {
                filteredServices = await goongDistanceService.GetBookableServicesWithinDistanceAsync(userPlaceId, filteredServices);
            }

            var serviceDTOs = filteredServices.Select(c => new CleaningServiceItemDTO
            {
                id = c.Id,
                name = c.ServiceName,
                category = c.Category.CategoryName,
                overallRating = c.Rating,
                price = c.Price,
                location = c.AddressLine,
                CategoryName = c.Category.CategoryName,
                Url = c.ServiceImages.FirstOrDefault()?.LinkUrl
            });

            return await PaginateResults(serviceDTOs, pageIndex, pageSize);
        }
        public async Task<ServiceFilterOptionsDTO> GetFilterOptionsAsync()
        {
            var serviceRepo = _unitOfWork.Repository<CleaningService>();

            var services = await serviceRepo.ListAsync(
                filter: s => s.Status == ServiceStatus.Active.ToString(),
                includeProperties: s => s.Include(c => c.Category).Include(c => c.ServiceRatings)
             );

            var categories = services
                .Select(s => new { s.Category.Id, s.Category.CategoryName })
                .Distinct()
                .Select(c => new CategoryFilterDTO { Id = c.Id, Name = c.CategoryName })
                .ToList();

            var minPrice = services.Min(s => s.Price);
            var maxPrice = services.Max(s => s.Price);

            var ratingOptions = new List<RatingFilterDTO>
            {
                new RatingFilterDTO { Range = 1, Count = services.Count(s => s.Rating >= 1 && s.Rating < 2) },
                new RatingFilterDTO { Range = 2, Count = services.Count(s => s.Rating >= 2 && s.Rating < 3) },
                new RatingFilterDTO { Range = 3, Count = services.Count(s => s.Rating >= 3 && s.Rating < 4) },
                new RatingFilterDTO { Range = 4, Count = services.Count(s => s.Rating >= 4 && s.Rating < 5) },
                new RatingFilterDTO { Range = 5, Count = services.Count(s => s.Rating == 5) }
            };

            return new ServiceFilterOptionsDTO
            {
                Categories = categories,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                RatingOptions = ratingOptions
            };
        }

        private async Task<CleaningServiceListDTO> PaginateResults(IEnumerable<CleaningServiceItemDTO> serviceDTOs, int? pageIndex, int? pageSize)
        {
            int totalCount = serviceDTOs.Count();

            if (!pageIndex.HasValue || !pageSize.HasValue || totalCount == 0)
            {
                serviceDTOs.AsQueryable();
                return new CleaningServiceListDTO
                {
                    Items = serviceDTOs.ToList(),
                    hasNext = false,
                    hasPrevious = false,
                    totalCount = totalCount,
                    totalPages = 1
                };
            }

            var paginatedItems = serviceDTOs
                .Skip((pageIndex.Value - 1) * pageSize.Value)
                .Take(pageSize.Value)
                .ToList();

            return new CleaningServiceListDTO
            {
                Items = paginatedItems,
                hasNext = (pageIndex.Value * pageSize.Value) < totalCount,
                hasPrevious = pageIndex.Value > 1,
                totalCount = totalCount,
                totalPages = (int)Math.Ceiling((double)totalCount / pageSize.Value)
            };
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
            var rating = await ratingService.GetRatingsByService(serviceId, 1, 1);
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
                reviews = rating == null ? rating.RatingAvg : 0,
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
                         review = rating.TotalCount > 0 ? rating.RatingAvg + " (" + rating.TotalCount + ")" : "No Review",
                         avatar = service.User.Avatar,
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
                filter: b => b.CleaningServiceId == serviceId && b.PreferDateStart.Date == targetDate.Date && b.Status == BookingStatus.OnGoing.ToString(),
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
                filter: c => c.UserId == userId && c.Status != "IsDeleted",
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

        public async Task<ServiceOverviewListDTO> GetServiceByUserFilter(string? status, ClaimsPrincipal userClaims, int? pageIndex, int? pageSize)
        {
            var userId = userClaims.FindFirst("id")?.Value;

            var services = _unitOfWork.Repository<CleaningService>()
                .GetAll()
                .Where(cs => cs.UserId == userId && cs.Status != "IsDeleted")
                .Include(cs => cs.ServiceImages)
                .Select(cs => new ServiceOverviewDTO
                {
                    AddressLine = cs.AddressLine,
                    Description = cs.Description,
                    Id = cs.Id,
                    Images = cs.ServiceImages.Select(si => new ImgDTO { id = si.Id, url = si.LinkUrl }).ToList(),
                    Name = cs.ServiceName,
                    NumOfBooking = cs.Bookings.Count(),
                    NumOfRatings = cs.RatingCount,
                    Rating = cs.Rating,
                    Status = cs.Status,
                    Duration = cs.Duration.ToString()
                });

            if (!string.IsNullOrEmpty(status))
            {
                if (!status.Contains(CommonConst.All, StringComparison.OrdinalIgnoreCase))
                {
                    services = services.Where(c => c.Status == status);
                }
            }

            if (services == null || !services.Any())
                return new ServiceOverviewListDTO
                {
                    Items = new()
                };

            if (pageIndex == null || pageSize == null)
            {
                var temp1 = await PaginatedList<ServiceOverviewDTO>.CreateAsync(services, 1, services.Count());
                return new ServiceOverviewListDTO
                {
                    Items = temp1,
                    HasNext = temp1.HasNextPage,
                    HasPrevious = temp1.HasPreviousPage,
                    TotalCount = temp1.TotalCount,
                    TotalPages = temp1.TotalPages
                };
            }
            var temp2 = await PaginatedList<ServiceOverviewDTO>.CreateAsync(services, (int)pageIndex, (int)pageSize);
            return new ServiceOverviewListDTO
            {
                Items = temp2,
                HasNext = temp2.HasNextPage,
                HasPrevious = temp2.HasPreviousPage,
                TotalCount = services.Count(),
                TotalPages = temp2.TotalPages
            };
        }

        public async Task<CreateCleaningServiceDTO?> CreateCleaningServiceAsync(CreateCleaningServiceDTO dto, ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException(CommonConst.UnauthorizeError);
            }
            var user = await _userManager.FindByIdAsync(userId) ?? throw new KeyNotFoundException(CommonConst.HousekeeperNotFound);

            //var newService = new CleaningService
            //{
            //    ServiceName = dto.ServiceName,
            //    CategoryId = dto.CategoryId,
            //    Description = dto.Description,
            //    Status = ServiceStatus.Pending.ToString(),
            //    Rating = 0,
            //    RatingCount = 0,
            //    Price = dto.Price,
            //    City = dto.City,
            //    District = dto.District,
            //    PlaceId = dto.PlaceId,
            //    AddressLine = dto.AddressLine,
            //    CreatedAt = DateTime.UtcNow,
            //    UpdatedAt = DateTime.UtcNow,
            //    Duration = Math.Round(dto.ServiceSteps.Sum(s => s.Duration) / 60.0, 2),                                  //Duration sẽ là tổng các step
            //    UserId = userId,
            //    User = await _userManager.FindByIdAsync(userId),
            //    Category = await _unitOfWork.Repository<ServiceCategory>().FindAsync(c => c.Id == dto.CategoryId)
            //};

            var housekeeperAddress = _unitOfWork.Repository<Address>().Find(c => c.UserId == user.Id && c.IsDefault == true);

            var newService = new CleaningService
            {
                ServiceName = dto.ServiceName,
                CategoryId = dto.CategoryId,
                Description = dto.Description,
                Status = ServiceStatus.Pending.ToString(),
                Rating = 0,
                RatingCount = 0,
                Price = dto.Price,
                City = housekeeperAddress!.City,
                District = housekeeperAddress.District,
                PlaceId = housekeeperAddress.PlaceId,
                AddressLine = housekeeperAddress.AddressLine1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Duration = Math.Round(dto.ServiceSteps.Sum(s => s.Duration) / 60.0, 2),                                  //Duration sẽ là tổng các step
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
                    Url = a.Url,
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
                    StepDescription = step.StepDescription,
                    Duration = step.Duration
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

            dto.Duration = Math.Round(dto.ServiceSteps.Sum(s => s.Duration) / 60.0, 2);
            return dto;
        }

        public async Task<CreateCleaningServiceDTO?> UpdateCleaningServiceAsync(Guid serviceId, CreateCleaningServiceDTO dto, ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException(CommonConst.UnauthorizeError);
            }

            var service = await _unitOfWork.Repository<CleaningService>().FindAsync(s => s.Id == serviceId && s.UserId == userId);
            if (service == null)
            {
                throw new KeyNotFoundException(CleaningServiceConst.ServiceNotFound);
            }
            service.ServiceName = dto.ServiceName;
            service.CategoryId = dto.CategoryId;
            service.Description = dto.Description;
            service.Price = dto.Price;
            service.Duration = Math.Round(dto.ServiceSteps.Sum(s => s.Duration) / 60.0, 2);                               //tính bằng tổng duration của step
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
                    StepDescription = step.StepDescription,
                    Duration = step.Duration
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
            dto.Duration = Math.Round(dto.ServiceSteps.Sum(s => s.Duration) / 60.0, 2);
            return dto;
        }

        public async Task<HousekeeperServiceDetailDTO?> GethousekeeperCleaningServiceDetailAsync(Guid serviceId, ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException(CommonConst.UnauthorizeError);
            }

            var service = await _unitOfWork.Repository<CleaningService>().FindAsync(s => s.Id == serviceId && s.UserId == userId);
            if (service == null)
            {
                throw new KeyNotFoundException(CleaningServiceConst.ServiceNotFound);
            }

            var additionalServices = await _unitOfWork.Repository<AdditionalService>().FindAllAsync(a => a.CleaningServiceId == serviceId);
            var serviceImages = await _unitOfWork.Repository<ServiceImg>().FindAllAsync(i => i.CleaningServiceId == serviceId);
            var serviceSteps = await _unitOfWork.Repository<ServiceSteps>().FindAllAsync(s => s.ServiceId == serviceId);
            var serviceTimeSlots = await _unitOfWork.Repository<ServiceTimeSlot>().FindAllAsync(t => t.ServiceId == serviceId);
            var serviceDistanceRules = await _unitOfWork.Repository<DistancePricingRule>().FindAllAsync(d => d.CleaningServiceId == serviceId);

            return new HousekeeperServiceDetailDTO
            {
                ServiceName = service.ServiceName,
                CategoryId = service.CategoryId,
                Description = service.Description,
                Price = service.Price.ToString(),
                City = service.City,
                District = service.District,
                PlaceId = service.PlaceId,
                AddressLine = service.AddressLine,
                Duration = service.Duration.ToString(),
                AdditionalServices = additionalServices.Select(a => new HousekeeperServiceDetailAdditionalServiceDTO
                {
                    Name = a.Name,
                    Amount = a.Amount.ToString(),
                    Url = a.Url,
                    Description = a.Description,
                    Duration = a.Duration.ToString()
                }).ToList(),
                ServiceImages = serviceImages.Select(i => new HousekeeperServiceDetailImgDTO
                {
                    LinkUrl = i.LinkUrl
                }).ToList(),
                ServiceSteps = serviceSteps.Select(s => new HousekeeperServiceDetailStepsDTO
                {
                    StepOrder = s.StepOrder,
                    StepDescription = s.StepDescription,
                    StepDuration = s.Duration.ToString(),
                }).ToList(),
                ServiceTimeSlots = serviceTimeSlots.Select(t => new HousekeeperServiceDetailTimeSlotDTO
                {
                    StartTime = t.StartTime,
                    DayOfWeek = t.DayOfWeek
                }).ToList(),
                ServiceDistanceRule = serviceDistanceRules.Select(d => new HousekeeperServiceDetailDistanceRuleDTO
                {
                    MinDistance = d.MinDistance.ToString(),
                    MaxDistance = d.MaxDistance.ToString(),
                    BaseFee = d.BaseFee.ToString(),
                    ExtraPerKm = d.ExtraPerKm.ToString()
                }).ToList()
            };
        }

        public async Task<List<HousekeeperSkillDTO>> GetHousekeeperCategories(ClaimsPrincipal housekeeper)
        {
            var housekeeperId = housekeeper.FindFirst("id")?.Value;
            if (housekeeperId == null)
            {
                throw new KeyNotFoundException(CommonConst.HousekeeperNotFound);
            }
            var currentHousekeeper = await _userManager.FindByIdAsync(housekeeperId);

            if (currentHousekeeper == null)
            {
                throw new Exception(CommonConst.DatabaseError);
            }

            var housekeeperSkills = await _unitOfWork.Repository<HousekeeperSkill>().GetAll().Where(s => s.HousekeeperId == housekeeperId).ToListAsync();

            return housekeeperSkills.Select(c => new HousekeeperSkillDTO
            {
                CategoryId = c.CategoryId,
                CategoryName = _unitOfWork.Repository<ServiceCategory>().GetById(c.CategoryId).CategoryName
            }).ToList();
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