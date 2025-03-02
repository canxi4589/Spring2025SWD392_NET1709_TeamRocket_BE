using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.GenericRepository;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.BookingDTO;
using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.Services.ListService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.Services.BookingService
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> userManager;

        public BookingService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            this.userManager = userManager;
        }
        public async Task<BookingHistoryResponseListDTO> GetBookingByUser(AppUser user, int? pageIndex, int? pageSize, string? status, int? day, int? month, int? year)
        {
            var bookingHistoryList = _unitOfWork.Repository<Booking>().GetAll().Where(c => c.Customer == user).Include(c => c.CleaningService).OrderByDescending(c => c.PreferDateStart);
            if (status.Equals("Recently")) bookingHistoryList.OrderByDescending(c => c.CreatedDate);
            if (status?.Equals("On-going", StringComparison.OrdinalIgnoreCase) ?? false && day.HasValue && month.HasValue && year.HasValue)
            {
                var targetDate = new DateTime(year.Value, month.Value, day.Value);
                bookingHistoryList = (IOrderedQueryable<Booking>)bookingHistoryList.Where(c => c.PreferDateStart.Date == targetDate);
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
                CleaningServiceDuration = c.CleaningService.Duration
            });
            if (pageIndex == null || pageSize == null)
            {
                var temp1 = await PaginatedList<BookingHistoryResponseDTO>.CreateAsync(bookingList, 1, bookingList.Count());
                return new BookingHistoryResponseListDTO
                {
                    Items = temp1,
                    hasNext = temp1.HasNextPage,
                    hasPrevious = temp1.HasPreviousPage,
                    totalCount = bookingList.Count(),
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
        public async Task<BookingHistoryDetailResponseDTO> GetBookingDetailById(Booking input)
        {
            var bookings = await _unitOfWork.Repository<Booking>().ListAsync(
                filter: c => c.Id == input.Id,
                includeProperties: query => query
                    .Include(c => c.CleaningService)
                    .Include(c => c.Payments)
                    .Include(c => c.BookingAdditionals).ThenInclude(c => c.AdditionalService)
                    .Include(c => c.Customer)
                    .Include(c => c.CleaningService.User)
            );

            var booking = bookings.FirstOrDefault();
            if (booking == null)
            {
                throw new Exception("Booking not found");
            }
            //var list = _unitOfWork.Repository<Booking>().GetAll().Where(c => c.Id == booking.Id).Include(c => c.CleaningService).Include(c => c.Payments).ToList();
            var bookingAdditional = _unitOfWork.Repository<BookingAdditional>().GetAll().Where(c => c.BookingId == booking.Id).ToList();
            var additionalService = _unitOfWork.Repository<AdditionalService>().GetAll().ToList();
            var additionalServiceNames = bookingAdditional.Select(b => additionalService.FirstOrDefault(c => c.Id == b.AdditionalServiceId)?.Name ?? "Unknown Service").ToList();
            var firstPayment = booking.Payments.FirstOrDefault();
            var customer = await userManager.FindByIdAsync(booking.CustomerId);
            var housekeeper = await userManager.FindByIdAsync(booking.CleaningService.UserId);

            return new BookingHistoryDetailResponseDTO
            {
                BookingId = booking.Id,
                PreferDateStart = booking.PreferDateStart,
                TimeStart = booking.TimeStart,
                TimeEnd = booking.TimeEnd,
                Status = booking.Status,
                TotalPrice = booking.TotalPrice,
                Note = booking.Note,
                Location = booking.AddressLine + " " + booking.District + " " + booking.City,
                ServiceName = booking.CleaningService.ServiceName,
                AdditionalServiceName = additionalServiceNames,
                HousekeeperName = housekeeper.FullName,
                HouseKeeperMail = housekeeper.Email,
                HouseKeeperPhoneNumber = housekeeper.PhoneNumber,
                CustomerName = customer.FullName,
                CustomerMail = customer.Email,
                CustomerPhoneNumber = customer.PhoneNumber,
                PaymentDate = firstPayment?.PaymentDate ?? DateTime.MinValue,
                PaymentMethod = firstPayment?.PaymentMethod ?? "Unknown",
                PaymentStatus = firstPayment?.Status ?? "Not found",
                CleaningServiceDuration = booking.CleaningService.Duration
            };
        }
        public async Task<CheckoutResponseDTO> GetCheckoutInfo(CheckoutRequestDTO request, ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirst("id")?.Value;
            var userEmail = userClaims.FindFirst("email")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var customer = await userManager.FindByIdAsync(userId);
            if (customer == null)
            {
                throw new KeyNotFoundException("User not found");
            }


            var service = await _unitOfWork.Repository<CleaningService>().GetEntityByIdAsync(request.ServiceId);
            if (service == null)
                throw new Exception("Service not found");

            var address = await _unitOfWork.Repository<Address>().GetEntityByIdAsync(request.AddressId);
            if (address == null)
                throw new Exception("Address not found");

            var bookingAdditionals = await _unitOfWork.Repository<AdditionalService>().ListAsync(
                ba => request.BookingAdditionalIds.Contains(ba.Id), orderBy: ba => ba.OrderBy(c => c.Id)
            );

            decimal additionalPrice =(decimal) bookingAdditionals.Sum(ba => ba.Amount);
            decimal totalPrice = service.Price + additionalPrice;

            var paymentMethods = new List<PaymentMethodDTO>
                {
                    new PaymentMethodDTO { Name = "Wallet",IsChoosable = false },
                    new PaymentMethodDTO { Name = "VNPay" ,IsChoosable = true }
                };

            var response = new CheckoutResponseDTO
            {
                UserName = customer.UserName,
                FullName = customer.FullName,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                City = address.City,
                District = address.District,
                AddressLine = address.AddressLine1,
                ServiceName = service.ServiceName,
                ServiceBasePrice = service.Price,
                Distance = "10 kms",
                DistancePrice = 10,
                AddidionalPrice = additionalPrice,
                TotalPrice = totalPrice,
                BookingAdditionalDTOs = bookingAdditionals.Select(ba => new BookingAdditionalDTO
                {
                    AdditionalId = ba.Id,
                    Name = ba.Name,
                    Price =(decimal) ba.Amount
                }).ToList(),
                PaymentMethods = paymentMethods.Select(pm => new PaymentMethodDTO { Name = pm.Name }).ToList()
            };

            return response;
        }

        //public async Task<Guid> CreateBooking(CheckoutResponseDTO checkoutDTO)
        //{
        //    if (bookingDTO == null)
        //    {
        //        throw new ArgumentNullException(nameof(bookingDTO), "Booking data is required.");
        //    }
        //    var customer = await userManager.FindByIdAsync(bookingDTO.CustomerId);
        //    if (customer == null)
        //    {
        //        throw new Exception("Customer not found.");
        //    }

        //    var cleaningService = await _unitOfWork.Repository<CleaningService>()
        //        .FindAsync(c => c.Id == bookingDTO.CleaningServiceId);
        //    if (cleaningService == null)
        //    {
        //        throw new Exception("Cleaning service not found.");
        //    }

        //    var timeSlot = await _unitOfWork.Repository<ServiceTimeSlot>()
        //        .FindAsync(t => t.Id == bookingDTO.TimeSlotId);
        //    if (timeSlot == null)
        //    {
        //        throw new Exception("Time slot not found.");
        //    }

        //    var booking = new Booking
        //    {
        //        Id = Guid.NewGuid(),
        //        PreferDateStart = DateTime.Now,
        //        TimeStart = timeSlot.StartTime,
        //        TimeEnd = timeSlot.EndTime,
        //        Status = BookingStatus.OnGoing.ToString(), 
        //        TotalPrice = bookingDTO.TotalPrice,
        //        CompletedAt = null,
        //        Customer = customer,
        //        CleaningService = cleaningService,
        //        Note = bookingDTO.Note,
        //    };
        //    foreach (var additionalDTO in bookingDTO.bookingAdditionalDTOs)
        //    {
        //        var additionalService = await _unitOfWork.Repository<AdditionalService>()
        //            .FindAsync(a => a.Id == additionalDTO.AdditionalId);

        //        if (additionalService == null)
        //        {
        //            throw new Exception($"Additional service with ID {additionalDTO.AdditionalId} not found.");
        //        }

        //        booking.BookingAdditionals.Add(new BookingAdditional
        //        {
        //            Id = Guid.NewGuid(),
        //            BookingId = booking.Id,
        //            AdditionalServiceId = additionalDTO.AdditionalId,
        //            Amount = additionalDTO.Amount,
        //            IsActive = true
        //        });
        //    }
        //    //_unitOfWork.Repository<Booking>().Insert(booking);
        //    //await _unitOfWork.SaveChangesAsync();

        //    return booking.Id; // Return booking ID for tracking
        //}


    }
}

