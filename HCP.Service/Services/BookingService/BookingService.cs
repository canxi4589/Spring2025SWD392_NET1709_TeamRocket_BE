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
        public async Task<BookingHistoryResponseListDTO> GetBookingByUser(AppUser user, int? pageIndex, int? pageSize)
        {
            var bookingHistoryList = _unitOfWork.Repository<Booking>().GetAll().Where(c => c.Customer == user).Include(c => c.CleaningService);
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
                totalCount = temp2.TotalCount,
                totalPages = temp2.TotalPages,
            };
        }
        public async Task<BookingHistoryDetailResponseDTO> GetBookingDetailById(Guid id)
        {
            var booking = _unitOfWork.Repository<Booking>().GetById(id);
            if (booking == null)
            {
                throw new Exception("Booking not found");
            }
            var bookingAdditional = _unitOfWork.Repository<BookingAdditional>().GetAll().Where(c => c.BookingId == id).ToList();
            var additionalService = _unitOfWork.Repository<AdditionalService>().GetAll().ToList();
            var additionalServiceNames = bookingAdditional.Select(b => additionalService.FirstOrDefault(c => c.Id == b.AdditionalServiceId)?.Name ?? "Unknown Service").ToList();

            var firstPayment = booking.Payments?.FirstOrDefault(); // Avoid multiple calls

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
                ServiceName = booking.CleaningService?.ServiceName ?? "No Service Available",
                AdditionalServiceName = additionalServiceNames,
                PaymentDate = firstPayment?.PaymentDate ?? DateTime.MinValue,
                PaymentMethod = firstPayment?.PaymentMethod ?? "Unknown",
                PaymentStatus = firstPayment?.Status ?? "Pending",
                CleaningServiceDuration = booking.CleaningService?.Duration ?? 0
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

            decimal additionalPrice = (decimal)bookingAdditionals.Sum(ba => ba.Amount);
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
                    Price = (decimal)ba.Amount
                }).ToList(),
                PaymentMethods = paymentMethods.Select(pm => new PaymentMethodDTO { Name = pm.Name }).ToList()
            };

            return response;
        }
        public async Task<Booking> GetBookingById(Guid id)
        {
            var bookingRepository =await _unitOfWork.Repository<Booking>().GetEntityByIdAsync(id);
            return bookingRepository;

        }
        public Booking UpdateStatusBooking(Guid id, string status)
        {
            var bookingRepository = _unitOfWork.Repository<Booking>();

            var booking =  bookingRepository.GetById(id);
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
            Guid id = Guid.NewGuid();
            var bookingAdditionals = bookingAdditionals1.Select(c => new BookingAdditional
            {
                BookingId = id,
                Amount = c.Amount,
                AdditionalServiceId = c.Id,
            });

            var booking = new Booking
            {
                Id = id,
                CustomerId = user.Id,
                CleaningServiceId = service.Id,
                PreferDateStart = dto.StartDate,
                TimeStart = timeSlot.StartTime,
                TimeEnd = timeSlot.EndTime,
                CreatedDate = DateTime.UtcNow,
                Status = BookingStatus.OnGoing.ToString(),
                TotalPrice = service.Price + (decimal)bookingAdditionals.Sum(a => a.Amount),
                ServicePrice = service.Price,
                DistancePrice = 0, // Calculate if needed
                AddtionalPrice = (decimal)bookingAdditionals.Sum(a => a.Amount),
                City = address.City,
                PlaceId = address.PlaceId,
                District = address.District,
                AddressLine = address.AddressLine1,
                Note = "",
                BookingAdditionals = bookingAdditionals.ToList()

            };

            await bookingRepository.AddAsync(booking);
            await _unitOfWork.Complete();

            return booking;
        }
    }
}




