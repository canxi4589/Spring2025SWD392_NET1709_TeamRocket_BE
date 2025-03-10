﻿using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.GenericRepository;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.BookingDTO;
using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.Services.ListService;
using Humanizer;
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
        private readonly IGoongDistanceService _goongDistanceService;
        public BookingService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IGoongDistanceService goongDistanceService)
        {
            _unitOfWork = unitOfWork;
            this.userManager = userManager;
            _goongDistanceService = goongDistanceService;
        }
        public async Task<BookingHistoryResponseListDTO> GetBookingByUser(AppUser user, int? pageIndex, int? pageSize, string? status, int? day, int? month, int? year)
        {
            var bookingHistoryList = _unitOfWork.Repository<Booking>().GetAll().Where(c => c.Customer == user).Include(c => c.CleaningService).OrderByDescending(c => c.PreferDateStart);
            if (status != null)
            {
                if (status.Equals("Recently")) bookingHistoryList.OrderByDescending(c => c.CreatedDate);
                if (day.HasValue && month.HasValue && year.HasValue)
                {
                    var targetDate = new DateTime(year.Value, month.Value, day.Value);
                    bookingHistoryList = (IOrderedQueryable<Booking>)bookingHistoryList.Where(c => c.PreferDateStart.Date == targetDate);
                }
                if (status.Equals("Ongoing"))
                {
                    bookingHistoryList = (IOrderedQueryable<Booking>)bookingHistoryList.Where(c => c.Status == BookingStatus.OnGoing.ToString());
                }
                if (status.Equals("Finished"))
                {
                    bookingHistoryList = (IOrderedQueryable<Booking>)bookingHistoryList.Where(c => c.Status == BookingStatus.Finished.ToString());
                }
                if (status.Equals("Canceled"))
                {
                    bookingHistoryList = (IOrderedQueryable<Booking>)bookingHistoryList.Where(c => c.Status == BookingStatus.Canceled.ToString());
                }
                if (status.Equals("Refunded"))
                {
                    bookingHistoryList = (IOrderedQueryable<Booking>)bookingHistoryList.Where(c => c.Status == BookingStatus.Refunded.ToString());
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
                    totalCount = temp1.TotalCount,
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

            decimal additionalPrice = (decimal)bookingAdditionals.Sum(ba => ba.Amount);
            double totalAdditionalDuration = bookingAdditionals.Sum(ba => ba.Duration ?? 0);

            double? distance = await _goongDistanceService.GetDistanceAsync(address.PlaceId, service.PlaceId);
            if (distance == null) throw new Exception("Failed to calculate distance");

            var pricingRule = await _unitOfWork.Repository<DistancePricingRule>().GetEntityAsync(
                rule => rule.CleaningServiceId == service.Id &&
                        rule.MinDistance <= distance &&
                        rule.MaxDistance >= distance &&
                        rule.IsActive
            );
            if (pricingRule == null)
                throw new Exception("Service is not available for this distance");

            decimal distancePrice = pricingRule.BaseFee;
            decimal totalPrice = service.Price + additionalPrice + distancePrice;

            var timeSlot = await _unitOfWork.Repository<ServiceTimeSlot>().GetEntityByIdAsync(request.TimeSlotId);
            if (timeSlot == null)
                throw new Exception("Time slot not found");

            bool isWalletChoosable =(decimal) customer.BalanceWallet >= totalPrice;

            var paymentMethods = new List<PaymentMethodDTO>
    {
        new PaymentMethodDTO { Name = "Wallet", IsChoosable = isWalletChoosable },
        new PaymentMethodDTO { Name = "VNPay", IsChoosable = true }
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
                Distance = $"{distance} km",
                DistancePrice = distancePrice,
                AddidionalPrice = additionalPrice,
                TotalPrice = totalPrice,
                TimeStart = timeSlot.StartTime,
                TimeEnd = timeSlot.EndTime + TimeSpan.FromMinutes(totalAdditionalDuration+30),
                BookingAdditionalDTOs = bookingAdditionals.Select(ba => new BookingAdditionalDTO
                {
                    AdditionalId = ba.Id,
                    Url = ba.Url,
                    Name = ba.Name,
                    Price = (decimal)ba.Amount,
                    Duration = ba.Duration
                }).ToList(),
                PaymentMethods = paymentMethods
            };

            return response;
        }
        public async Task<Booking> GetBookingById(Guid id)
        {
            var bookingRepository = await _unitOfWork.Repository<Booking>().GetEntityByIdAsync(id);
            return bookingRepository;

        }
        public Booking UpdateStatusBooking(Guid id, string status)
        {

            var bookingRepository = _unitOfWork.Repository<Booking>();
            var paymentRepository = _unitOfWork.Repository<Payment>();
            var payment = paymentRepository.GetAll().FirstOrDefault(c => c.BookingId == id);

            if (payment == null)
                throw new KeyNotFoundException("Payment record not found");

            payment.Status = "Failed";

            var booking = bookingRepository.GetById(id);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found.");
            }

            booking.Status = status;

            bookingRepository.Update(booking);
            _unitOfWork.Complete();

            return booking;
        }
        public async Task DeleteBooking(Guid id)
        {
            var bookingRepository = _unitOfWork.Repository<Booking>();

            var booking = await bookingRepository.GetEntityByIdAsync(id);
            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found.");
            }

            bookingRepository.Delete(booking);
            await _unitOfWork.Complete(); // Save changes
        }

        public async Task<Booking> CreateBookingAsync(ConfirmBookingDTO dto, ClaimsPrincipal userClaims)
        {
            var bookingRepository = _unitOfWork.Repository<Booking>();
            var serviceRepository = _unitOfWork.Repository<CleaningService>();
            var timeSlotRepository = _unitOfWork.Repository<ServiceTimeSlot>();
            var addressRepository = _unitOfWork.Repository<Address>();
            var bookingAdditionalRepository = _unitOfWork.Repository<BookingAdditional>();
            var paymentRepository = _unitOfWork.Repository<Payment>();
            var distancePricingRepository = _unitOfWork.Repository<DistancePricingRule>();

            var userId = userClaims.FindFirst("id")?.Value;
            var userEmail = userClaims.FindFirst("email")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            var service = await serviceRepository.GetEntityByIdAsync(dto.ServiceId);
            if (service == null)
                throw new Exception("Service not found");

            var timeSlot = await timeSlotRepository.GetEntityByIdAsync(dto.TimeSlotId);
            if (timeSlot == null)
                throw new Exception("Time slot not found");

            var address = await addressRepository.GetEntityByIdAsync(dto.AddressId);
            if (address == null)
                throw new Exception("Address not found");

            var bookingAdditionals1 = await _unitOfWork.Repository<AdditionalService>().ListAsync(
                ba => dto.BookingAdditionalIds.Contains(ba.Id), orderBy: ba => ba.OrderBy(c => c.Id)
            );
            double? distance = await _goongDistanceService.GetDistanceAsync(address.PlaceId, service.PlaceId);
            if (distance == null) throw new Exception("Failed to calculate distance");

            var pricingRule = await distancePricingRepository.GetEntityAsync(
                rule => rule.CleaningServiceId == service.Id &&
                        rule.MinDistance <= distance &&
                        rule.MaxDistance >= distance &&
                        rule.IsActive
            );

            if (pricingRule == null)
                throw new Exception("Service is not available for this distance");

            decimal distancePrice = pricingRule.BaseFee;

            Guid id = Guid.NewGuid();
            var bookingAdditionals = bookingAdditionals1.Select(c => new BookingAdditional
            {
                BookingId = id,
                Amount = c.Amount,
                AdditionalServiceId = c.Id,
            });

            var totalAdditionalDuration = bookingAdditionals1.Sum(a => a.Duration);
            var booking = new Booking
            {
                Id = id,
                CustomerId = user.Id,
                CleaningServiceId = service.Id,
                PreferDateStart = dto.StartDate,
                TimeStart = timeSlot.StartTime,
                TimeEnd = timeSlot.EndTime + TimeSpan.FromMinutes((double)totalAdditionalDuration+30),
                CreatedDate = DateTime.UtcNow,
                Status = BookingStatus.OnGoing.ToString(),
                TotalPrice = service.Price + (decimal)bookingAdditionals.Sum(a => a.Amount),
                ServicePrice = service.Price,
                DistancePrice = distancePrice,
                AddtionalPrice = (decimal)bookingAdditionals.Sum(a => a.Amount),
                City = address.City,
                PlaceId = address.PlaceId,
                District = address.District,
                AddressLine = address.AddressLine1,
                Note = dto.Note ?? "",
                BookingAdditionals = bookingAdditionals.ToList()
            };


            await bookingRepository.AddAsync(booking);
            await _unitOfWork.Complete();

            return booking;
        }


        public async Task<Payment> CreatePayment(Guid bookingId, decimal amount, string paymentMethod = "VNPay")
        {
            var paymentRepository = _unitOfWork.Repository<Payment>();

            var payment = new Payment
            {
                BookingId = bookingId,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = paymentMethod,
                Status = "succeed",
                Amount = amount
            };

            await paymentRepository.AddAsync(payment);
            await _unitOfWork.Complete();

            return payment;
        }
        public async Task<Payment> UpdatePaymentStatusAsync(Guid paymentId, string status)
        {
            var paymentRepository = _unitOfWork.Repository<Payment>();
            var payment = await paymentRepository.GetEntityByIdAsync(paymentId);

            if (payment == null)
                throw new KeyNotFoundException("Payment record not found");

            payment.Status = status;
            paymentRepository.Update(payment);
            await _unitOfWork.Complete();
            return payment;

        }

        public async Task<BookingListResponseDto> GetHousekeeperBookingsAsync(ClaimsPrincipal userClaims, int page, int pageSize)
        {
            var bookingRepository = _unitOfWork.Repository<Booking>();
            var userId = userClaims.FindFirst("id")?.Value;

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var bookingsQuery = await _unitOfWork.Repository<Booking>().ListAsync(
                filter: c => c.CleaningService.UserId == userId,
                includeProperties: query => query
                    .Include(c => c.CleaningService)
                    .Include(c => c.Payments)
                    .Include(c => c.BookingAdditionals).ThenInclude(c => c.AdditionalService)
                    .Include(c => c.Customer)
            );
            int totalCount = bookingsQuery.Count();

            var bookings = bookingsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListItemDto
                {
                    Id = b.Id,
                    PreferDateStart = b.PreferDateStart,
                    TimeStart = b.TimeStart,
                    TimeEnd = b.TimeEnd,
                    Status = b.Status,
                    IsFinishable = b.Status == BookingStatus.OnGoing.ToString(),
                    TotalPrice = b.TotalPrice,
                    ServicePrice = b.ServicePrice,
                    DistancePrice = b.DistancePrice,
                    AdditionalPrice = b.AddtionalPrice,
                    ServiceName = b.CleaningService.ServiceName,
                    ServiceDescription = b.CleaningService.Description,
                    ServiceImageUrl = b.CleaningService.ServiceImages.FirstOrDefault()?.LinkUrl,
                    Customer = new CustomerDto
                    {
                        FullName = b.Customer.FullName,
                        Email = b.Customer.Email,
                        PhoneNumber = b.Customer.PhoneNumber,
                        AvatarUrl = b.Customer.Avatar
                    },
                    AddressLine1 = b.AddressLine,
                    City = b.City,
                    District = b.District
                })
                .ToList();

            return new BookingListResponseDto { Items = bookings, TotalCount = totalCount };
        }



    }
}




