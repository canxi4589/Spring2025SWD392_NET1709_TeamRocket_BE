using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.AdminManagementDTO;
using HCP.Service.Services.ListService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using static HCP.Service.DTOs.AdminManagementDTO.ChartDataAdminDTO;
using System.Globalization;
using static HCP.Service.DTOs.AdminManagementDTO.ServiceAdminDTO;
using static HCP.Service.DTOs.AdminManagementDTO.ServiceCategoryAdminDTO;
using HCP.Repository.Enums;
using HCP.Service.DTOs.BookingDTO;
using System.Security.Claims;

namespace HCP.Service.Services.AdminManService
{
    public class AdminManService : IAdminManService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AdminManService(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        private async Task<List<UserAdminDTO>> GetUsersByRoleAsync(string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            return users.Select(user => new UserAdminDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Status = "IsActive",
                Birthday = user.Birthday,
                Avatar = user.Avatar
            }).ToList();
        }
        public async Task<UserAdminListDTO> GetUsersByAdminCustom(string? search, bool includeStaff, bool includeCustomers, bool includeHousekeepers, int? pageIndex, int? pageSize)
        {
            var users = new List<UserAdminDTO>();
            if (includeStaff)
            {
                users.AddRange(await GetUsersByRoleAsync("Staff"));
            }
            if (includeCustomers)
            {
                users.AddRange(await GetUsersByRoleAsync("Customer"));
            }
            if (includeHousekeepers)
            {
                users.AddRange(await GetUsersByRoleAsync("Housekeeper"));
            }
            if (!search.IsNullOrEmpty())
            {
                users = users.Where(c => (c.FullName.ToLower().Contains(search.ToLower())) || (c.Email.ToLower().Contains(search.ToLower()))).ToList();
            }
            if (pageIndex == null || pageSize == null)
            {
                var temp1 = PaginatedList<UserAdminDTO>.CreateAsync(users, 1, users.Count());
                return new UserAdminListDTO
                {
                    Items = temp1,
                    hasNext = temp1.HasNextPage,
                    hasPrevious = temp1.HasPreviousPage,
                    totalCount = users.Count,
                    totalPages = temp1.TotalPages,
                };
            }
            var temp2 = PaginatedList<UserAdminDTO>.CreateAsync(users, (int)pageIndex, (int)pageSize);
            return new UserAdminListDTO
            {
                Items = temp2,
                hasNext = temp2.HasNextPage,
                hasPrevious = temp2.HasPreviousPage,
                totalCount = users.Count(),
                totalPages = temp2.TotalPages,
            };
        }
        public async Task<ServiceCategoryAdminShowListDTO> GetAllServiceCategoriesAsync(string? search, int? pageIndex, int? pageSize, int? day, int? month, int? year)
        {
            var categories = _unitOfWork.Repository<ServiceCategory>().GetAll().Include(c => c.CleaningServices).ThenInclude(c => c.Bookings);
            var categorieList = categories.Select(category => new ServiceCategoryAdminShowDTO
            {
                CategoryName = category.CategoryName,
                CategoryId = category.Id,
                PictureUrl = category.PictureUrl,
                CreatedAt = category.CreatedAt,
                UpdatedAt = category.UpdatedAt,
                Description = category.Description,
                NumberOfBooking = category.CleaningServices.Sum(cs => cs.Bookings.Count)
            });
            if (!search.IsNullOrEmpty())
            {
                categorieList = categorieList.Where(c => (c.CategoryName.ToLower().Contains(search.ToLower()))
                || (c.CategoryName.ToLower().Contains(search.ToLower())) || (c.Description.ToLower().Contains(search.ToLower())));
            }
            if (day.HasValue && month.HasValue && year.HasValue)
            {
                categorieList = categorieList.Where(c => (c.CreatedAt.Day == day && c.CreatedAt.Month == month && c.CreatedAt.Year == year));
            }
            if (pageIndex == null || pageSize == null)
            {
                var temp1 = await PaginatedList<ServiceCategoryAdminShowDTO>.CreateAsync(categorieList, 1, categorieList.Count());
                return new ServiceCategoryAdminShowListDTO
                {
                    Items = temp1,
                    hasNext = temp1.HasNextPage,
                    hasPrevious = temp1.HasPreviousPage,
                    totalCount = categorieList.Count(),
                    totalPages = temp1.TotalPages,
                };
            }
            var temp2 = await PaginatedList<ServiceCategoryAdminShowDTO>.CreateAsync(categorieList, (int)pageIndex, (int)pageSize);
            return new ServiceCategoryAdminShowListDTO
            {
                Items = temp2,
                hasNext = temp2.HasNextPage,
                hasPrevious = temp2.HasPreviousPage,
                totalCount = categorieList.Count(),
                totalPages = temp2.TotalPages,
            };
        }
        public async Task<ServiceAdminShowListDTO> GetAllServicesAsync(string? search, int? pageIndex, int? pageSize, int? day, int? month, int? year)
        {
            var services = await _unitOfWork.Repository<CleaningService>()
                .GetAll()
                .Include(c => c.User)
                .Include(c => c.ServiceImages).Include(c => c.Bookings)
                .ToListAsync();

            var svList = new List<ServiceAdminShowDTO>();
            foreach (var service in services)
            {
                var staff = await _userManager.FindByIdAsync(service.StaffId ?? string.Empty);
                svList.Add(new ServiceAdminShowDTO
                {
                    Id = service.Id,
                    ServiceName = service.ServiceName,
                    CategoryId = service.CategoryId,
                    CategoryName = service.Category?.CategoryName ?? "Not found",
                    Status = service.Status,
                    Rating = service.Rating,
                    RatingCount = service.RatingCount,
                    Price = service.Price,
                    City = service.City,
                    District = service.District,
                    AddressLine = service.AddressLine,
                    PlaceId = service.PlaceId,
                    CreatedAt = service.CreatedAt,
                    UpdatedAt = service.UpdatedAt,
                    Duration = service.Duration,
                    HousekeeperId = service.UserId,
                    HousekeeperName = service.User?.FullName ?? "Not found",
                    StaffId = service.StaffId ?? "Not found",
                    StaffName = staff?.FullName ?? "Not found",
                    FirstImgLinkUrl = service.ServiceImages.FirstOrDefault()?.LinkUrl,
                    NumberOfBooking = service.Bookings.Count()
                });
            }
            if (!search.IsNullOrEmpty())
            {
                svList = svList.Where(c => (c.ServiceName.ToLower().Contains(search.ToLower()))
                || (c.CategoryName.ToLower().Contains(search.ToLower()))
                || (c.City.ToLower().Contains(search.ToLower()))
                || (c.District.ToLower().Contains(search.ToLower()))
                || (c.AddressLine.ToLower().Contains(search.ToLower()))
                || (c.HousekeeperName.ToLower().Contains(search.ToLower()))
                || (c.StaffName.ToLower().Contains(search.ToLower()))
                || (c.HousekeeperName.ToLower().Contains(search.ToLower()))
                || (c.Price.ToString().ToLower().Contains(search.ToLower()))
                ).ToList();
            }
            if (day.HasValue && month.HasValue && year.HasValue)
            {
                svList = svList.Where(c => (c.CreatedAt.Day == day && c.CreatedAt.Month == month && c.CreatedAt.Year == year)).ToList();
            }
            if (pageIndex == null || pageSize == null)
            {
                var temp1 = PaginatedList<ServiceAdminShowDTO>.Create(svList.AsQueryable(), 1, svList.Count);
                return new ServiceAdminShowListDTO
                {
                    Items = temp1,
                    hasNext = temp1.HasNextPage,
                    hasPrevious = temp1.HasPreviousPage,
                    totalCount = svList.Count,
                    totalPages = temp1.TotalPages,
                };
            }
            var temp2 = PaginatedList<ServiceAdminShowDTO>.Create(svList.AsQueryable(), (int)pageIndex, (int)pageSize);
            return new ServiceAdminShowListDTO
            {
                Items = temp2,
                hasNext = temp2.HasNextPage,
                hasPrevious = temp2.HasPreviousPage,
                totalCount = svList.Count,
                totalPages = temp2.TotalPages,
            };
        }
        //Get data for admin category chart base on completed date of booking and booking status and payment status 
        public async Task<ChartCategoryDataAdminShowListDTO> GetCategoryChart(
    bool dayChart, bool yearChart, bool yearsChart,
    int? dayStart, int? monthStart, int? yearStart,
    int? dayEnd, int? monthEnd, int? yearEnd)
        {
            var categories = await _unitOfWork.Repository<ServiceCategory>().GetAll()
                .Include(c => c.CleaningServices)
                .ThenInclude(cs => cs.Bookings)
                .ThenInclude(b => b.Payments)
                .ToListAsync();
            var chartDataList = new List<ChartCategoryDataAdminShowDTO>();
            DateTime? startDate = null;
            DateTime? endDate = null;

            if (yearChart && yearStart.HasValue)
            {
                // For year chart, cover the entire year
                startDate = new DateTime(yearStart.Value, 1, 1);
                endDate = new DateTime(yearStart.Value, 12, 31);
            }
            else if (yearsChart && yearStart.HasValue && yearEnd.HasValue)
            {
                // For years chart, cover the range of years
                startDate = new DateTime(yearStart.Value, 1, 1);
                endDate = new DateTime(yearEnd.Value, 12, 31);
            }
            else if (dayChart && !yearChart && !yearsChart &&
                     dayStart.HasValue && monthStart.HasValue && yearStart.HasValue &&
                     dayEnd.HasValue && monthEnd.HasValue && yearEnd.HasValue)
            {
                // For day chart, cover the specified date range
                startDate = new DateTime(yearStart.Value, monthStart.Value, dayStart.Value);
                endDate = new DateTime(yearEnd.Value, monthEnd.Value, dayEnd.Value);
            }

            // Create the category list based on the date range (or no range for default case)
            if (startDate.HasValue && endDate.HasValue)
            {
                chartDataList = categories.Select(category => new ChartCategoryDataAdminShowDTO
                {
                    name = category.CategoryName,
                    numberOfBookings = category.CleaningServices
                        .SelectMany(cs => cs.Bookings)
                        .Count(b => b.Status == BookingStatus.Completed.ToString() &&
                                   b.CompletedAt.HasValue &&
                                   b.CompletedAt.Value >= startDate.Value &&
                                   b.CompletedAt.Value <= endDate.Value &&
                                   b.Payments.Any(p => p.Status == "succeed"))
                }).ToList();
            }
            else
            {
                // Default case: no specific time filter, count all completed bookings with successful payments
                chartDataList = categories.Select(category => new ChartCategoryDataAdminShowDTO
                {
                    name = category.CategoryName,
                    numberOfBookings = category.CleaningServices
                        .SelectMany(cs => cs.Bookings)
                        .Count(b => b.Status == BookingStatus.Completed.ToString() &&
                                   b.CompletedAt.HasValue &&
                                   b.Payments.Any(p => p.Status == "succeed"))
                }).ToList();
            }

            return new ChartCategoryDataAdminShowListDTO
            {
                ChartData = chartDataList
            };
        }
        //Get data for admin revenue chart base on completed date of booking and booking status and payment status 
        public async Task<ChartDataAdminShowListDTO> GetRevenueChartDatas(bool dayChart, bool weekChart, bool yearChart, bool yearsChart, int? dayStart, int? monthStart, int? yearStart, int? dayEnd, int? monthEnd, int? yearEnd)
        {
            var chartDataList = new List<ChartDataAdminShowDTO>();

            // Query payments with status "Completed" and include booking
            var payments = _unitOfWork.Repository<Payment>()
                .GetAll()
                .Where(p => p.Status == "succeed" && p.Booking.Status.Equals(BookingStatus.Completed.ToString()))
                .Include(p => p.Booking)
                .ToList();

            if (yearChart && yearStart.HasValue)
            {
                // Year chart - show data for each month in the selected year
                for (int month = 1; month <= 12; month++)
                {
                    // Define the start and end of the month
                    var monthStartDate = new DateTime(yearStart.Value, month, 1);
                    var monthEndDate = monthStartDate.AddMonths(1).AddDays(-1);
                    var monthlyRevenue = payments.Where(p => p.Booking.CompletedAt >= monthStartDate && p.Booking.CompletedAt <= monthEndDate).Sum(p => p.Amount) * 0.1m; // 10% of the amount
                    chartDataList.Add(new ChartDataAdminShowDTO
                    {
                        name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                        revenue = (double)monthlyRevenue
                    });
                }
            }

            else if (yearsChart && yearStart.HasValue && yearEnd.HasValue)
            {
                // Years chart - show data for each year between start and end year
                for (int year = yearStart.Value; year <= yearEnd.Value; year++)
                {
                    var yearStartDate = new DateTime(year, 1, 1);
                    var yearEndDate = new DateTime(year, 12, 31);

                    var yearlyRevenue = payments
                        .Where(p => p.Booking.CompletedAt >= yearStartDate && p.Booking.CompletedAt <= yearEndDate)
                        .Sum(p => p.Amount) * 0.1m; // 10% of the amount

                    chartDataList.Add(new ChartDataAdminShowDTO
                    {
                        name = year.ToString(),
                        revenue = (double)yearlyRevenue
                    });
                }
            }
            else if (dayChart && !weekChart && !yearChart && !yearsChart &&
                     dayStart.HasValue && monthStart.HasValue && yearStart.HasValue &&
                     dayEnd.HasValue && monthEnd.HasValue && yearEnd.HasValue)
            {
                // Day chart - show data for each day between start and end date
                var startDate = new DateTime(yearStart.Value, monthStart.Value, dayStart.Value);
                var endDate = new DateTime(yearEnd.Value, monthEnd.Value, dayEnd.Value);

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var dailyRevenue = payments
                        .Where(p => p.Booking.CompletedAt.HasValue && p.Booking.CompletedAt.Value.Date == date.Date)
                        .Sum(p => p.Amount) * 0.1m; // 10% of the amount

                    chartDataList.Add(new ChartDataAdminShowDTO
                    {
                        name = date.ToString("dd/MM/yyyy"),
                        revenue = (double)dailyRevenue
                    });
                }
            }
            return new ChartDataAdminShowListDTO
            {
                ChartData = chartDataList
            };
        }
        //Get all booking base on preferdate
        public async Task<BookingHistoryResponseListDTO> GetAllBookingByCateAndServiceAdmin(bool isService, bool isCategory, Guid Id, int? pageIndex, int? pageSize, string? status, int? day, int? month, int? year)
        {

            var bookingHistoryList = _unitOfWork.Repository<Booking>().GetAll().Include(c => c.CleaningService).ThenInclude(c => c.Category).OrderByDescending(c => c.PreferDateStart)
                .Where(c => (isCategory && c.CleaningService.Category.Id == Id) || (isService && c.CleaningService.Id == Id));
            if (day.HasValue && month.HasValue && year.HasValue)
            {
                //var targetDay = new DateTime(day.Value, year.Value, month.
                bookingHistoryList = (IOrderedQueryable<Booking>)bookingHistoryList
                    .Where(c => (c.PreferDateStart.Day == day && c.PreferDateStart.Month == month && c.PreferDateStart.Year == year));
            }
            if (status != null)
            {
                if (status.Equals("Recently")) bookingHistoryList.OrderByDescending(c => c.CreatedDate);
                if (status.Equals(BookingStatus.OnGoing.ToString()))
                {
                    bookingHistoryList = (IOrderedQueryable<Booking>)bookingHistoryList.Where(c => c.Status == BookingStatus.OnGoing.ToString());
                }
                if (status.Equals(BookingStatus.Canceled.ToString()))
                {
                    bookingHistoryList = (IOrderedQueryable<Booking>)bookingHistoryList.Where(c => c.Status == BookingStatus.Canceled.ToString());
                }
                if (status.Equals(BookingStatus.Refunded.ToString()))
                {
                    bookingHistoryList = (IOrderedQueryable<Booking>)bookingHistoryList.Where(c => c.Status == BookingStatus.Refunded.ToString());
                }
                if (status.Equals(BookingStatus.Completed.ToString()))
                {
                    bookingHistoryList = (IOrderedQueryable<Booking>)bookingHistoryList.Where(c => c.Status == BookingStatus.Completed.ToString());
                }
            }
            var bookingList = bookingHistoryList.Select(c => new BookingHistoryResponseDTO
            {
                BookingId = c.Id,
                PreferDateStart = c.PreferDateStart,
                TimeStart = c.TimeStart,
                TimeEnd = c.TimeEnd,
                Status = c.Status,
                TotalPrice = c.TotalPrice,
                Note = c.Note,
                Location = c.AddressLine + ", " + c.District + ", " + c.City,
                ServiceName = c.CleaningService.ServiceName,
                CleaningServiceDuration = c.CleaningService.Duration,
                isRating = c.isRating,
                CleaningServiceId = c.CleaningServiceId
            });
            if (pageIndex == null || pageSize == null)
            {
                var temp1 = await PaginatedList<BookingHistoryResponseDTO>.CreateAsync(bookingList, 1, bookingList.Count());
                return new BookingHistoryResponseListDTO
                {
                    Items = temp1,
                    hasNext = temp1.HasNextPage,
                    hasPrevious = temp1.HasPreviousPage,
                    totalCount = temp1.Count(),
                    totalPages = temp1.TotalPages,
                };
            }
            var temp2 = await PaginatedList<BookingHistoryResponseDTO>.CreateAsync(bookingList, (int)pageIndex, (int)pageSize);
            return new BookingHistoryResponseListDTO
            {
                Items = temp2,
                hasNext = temp2.HasNextPage,
                hasPrevious = temp2.HasPreviousPage,
                totalCount = bookingList.Count(),
                totalPages = temp2.TotalPages,
            };
        }
    }
}
