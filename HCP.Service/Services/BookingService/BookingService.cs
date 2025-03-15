using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.GenericRepository;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.BookingDTO;
using HCP.Service.DTOs.CheckoutDTO;
using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.Services.CustomerService;
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
        private readonly ICustomerService _customerService;
        public BookingService(ICustomerService customerService, IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IGoongDistanceService goongDistanceService)
        {
            _unitOfWork = unitOfWork;
            this.userManager = userManager;
            _goongDistanceService = goongDistanceService;
            _customerService = customerService;
        }
        public async Task<BookingHistoryResponseListDTO> GetBookingByUser(AppUser user, int? pageIndex, int? pageSize, string? status, int? day, int? month, int? year)
        {
            var bookingHistoryList = _unitOfWork.Repository<Booking>().GetAll().Where(c => c.Customer == user).Include(c => c.CleaningService).OrderByDescending(c => c.PreferDateStart);
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
                throw new Exception(CommonConst.NotFoundError);
            }
            //var list = _unitOfWork.Repository<Booking>().GetAll().Where(c => c.Id == booking.Id).Include(c => c.CleaningService).Include(c => c.Payments).ToList();
            var bookingAdditional = _unitOfWork.Repository<BookingAdditional>().GetAll().Where(c => c.BookingId == booking.Id).ToList();
            var additionalService = _unitOfWork.Repository<AdditionalService>().GetAll().ToList();
            var additionalServiceNames = bookingAdditional.Select(b => additionalService.FirstOrDefault(c => c.Id == b.AdditionalServiceId)?.Name ?? KeyConst.Unknown).ToList();
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
                PaymentMethod = firstPayment?.PaymentMethod ?? KeyConst.Unknown,
                PaymentStatus = firstPayment?.Status ?? CommonConst.NotFoundError,
                CleaningServiceDuration = booking.CleaningService.Duration,
                isRating = booking.isRating,
                CleaningServiceId = booking.CleaningService.Id
            };
        }
        public async Task<CheckoutResponseDTO> GetCheckoutInfo(CheckoutRequestDTO request, ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException(CommonConst.UnauthorizeError);
            }

            var customer = await userManager.FindByIdAsync(userId);
            if (customer == null)
            {
                throw new KeyNotFoundException(CustomerConst.NotFoundError);
            }

            var service = await _unitOfWork.Repository<CleaningService>().GetEntityByIdAsync(request.ServiceId);
            if (service == null)
                throw new Exception(CleaningServiceConst.ServiceNotFound);

            var address = await _unitOfWork.Repository<Address>().GetEntityByIdAsync(request.AddressId);
            if (address == null)
                throw new Exception(CommonConst.NotFoundError);

            var bookingAdditionals = await _unitOfWork.Repository<AdditionalService>().ListAsync(
                ba => request.BookingAdditionalIds.Contains(ba.Id), orderBy: ba => ba.OrderBy(c => c.Id)
            );

            decimal additionalPrice = (decimal)bookingAdditionals.Sum(ba => ba.Amount);
            double totalAdditionalDuration = bookingAdditionals.Sum(ba => ba.Duration ?? 0);

            double? distance = await _goongDistanceService.GetDistanceAsync(address.PlaceId, service.PlaceId);
            if (distance == null) throw new Exception(CommonConst.SomethingWrongMessage);

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

            bool isWalletChoosable = (decimal)customer.BalanceWallet >= totalPrice;

            var paymentMethods = new List<PaymentMethodDTO1>
    {
        new PaymentMethodDTO1 { Name = KeyConst.Wallet, IsChoosable = isWalletChoosable },
        new PaymentMethodDTO1 { Name = KeyConst.VNPay, IsChoosable = true }
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
                TimeEnd = timeSlot.EndTime + TimeSpan.FromMinutes(totalAdditionalDuration + 30),
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

        public async Task<BookingCountDTO> GetBookingCountHousekeeper(ClaimsPrincipal claim)
        {
            var user = await _customerService.GetCustomerAsync(claim);
            var bookingRepository = _unitOfWork.Repository<Booking>().GetAll()
                .Include(c => c.CleaningService).Where(s => s.CleaningService.UserId == user.Id).ToList();
            return new BookingCountDTO
            {
                UpcomingBookings = bookingRepository.Where(b => b.Status.Equals(BookingStatus.OnGoing.ToString())).Count(),
                CompletedBookings = bookingRepository.Where(b => b.Status.Equals(BookingStatus.Completed.ToString())).Count(),
                CanceledBookings = bookingRepository.Where(b => b.Status.Equals(BookingStatus.Canceled.ToString())).Count(),
                RefundedBookings = bookingRepository.Where(b => b.Status.Equals(BookingStatus.Refunded.ToString())).Count()
            };
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

        public async Task<Booking> CreateBookingAsync(CheckoutResponseDTO1 dto, ClaimsPrincipal userClaims)
        {
            var bookingRepository = _unitOfWork.Repository<Booking>();
            var serviceRepository = _unitOfWork.Repository<CleaningService>();
            var timeSlotRepository = _unitOfWork.Repository<ServiceTimeSlot>();
            var addressRepository = _unitOfWork.Repository<Address>();
            var distancePricingRepository = _unitOfWork.Repository<DistancePricingRule>();

            var userId = userClaims.FindFirst("id")?.Value ?? dto.CustomerId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var service = await serviceRepository.GetEntityByIdAsync(dto.CleaningServiceId);
            if (service == null)
                throw new Exception("Service not found");

            var timeSlot = await timeSlotRepository.GetEntityByIdAsync(dto.TimeSlotId);
            if (timeSlot == null)
                throw new Exception("Time slot not found");

            var address = await addressRepository.GetEntityByIdAsync(dto.AddressId);
            if (address == null)
                throw new Exception("Address not found");

            var bookingAdditionals1 = await _unitOfWork.Repository<AdditionalService>().ListAsync(
                ba => dto.AdditionalServices.Select(a => a.AdditionalServiceId).Contains(ba.Id),
                orderBy: ba => ba.OrderBy(c => c.Id)
            );

            double? distance = await _goongDistanceService.GetDistanceAsync(address.PlaceId, service.PlaceId);
            if (distance == null)
                throw new Exception("Failed to calculate distance");

            var pricingRule = await distancePricingRepository.GetEntityAsync(
                rule => rule.CleaningServiceId == service.Id &&
                        rule.MinDistance <= distance &&
                        rule.MaxDistance >= distance &&
                        rule.IsActive
            );

            if (pricingRule == null)
                throw new Exception("Service is not available for this distance");

            decimal distancePrice = pricingRule.BaseFee;

            var bookingAdditionals = bookingAdditionals1.Select(c => new BookingAdditional
            {
                BookingId = dto.Id,
                Amount = (double)dto.AdditionalServices.FirstOrDefault(a => a.AdditionalServiceId == c.Id).Amount ,
                AdditionalServiceId = c.Id,
            }).ToList();

            var totalAdditionalDuration = bookingAdditionals1.Sum(a => a.Duration);
            var booking = new Booking
            {
                Id = dto.Id,
                CustomerId = user.Id,
                CleaningServiceId = service.Id,
                PreferDateStart = dto.BookingDate,
                TimeStart = timeSlot.StartTime,
                TimeEnd = timeSlot.EndTime + TimeSpan.FromMinutes((double)totalAdditionalDuration + 30),
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
                Note = /*dto.Note ??*/ "",
                BookingAdditionals = bookingAdditionals,
                
            };

            await bookingRepository.AddAsync(booking);
            await _unitOfWork.Complete();

            return booking;
        }
        public async Task<Booking> CreateBookingAsync1(CheckoutResponseDTO1 dto, string uid)
        {
            var bookingRepository = _unitOfWork.Repository<Booking>();
            var serviceRepository = _unitOfWork.Repository<CleaningService>();
            var timeSlotRepository = _unitOfWork.Repository<ServiceTimeSlot>();
            var addressRepository = _unitOfWork.Repository<Address>();
            var distancePricingRepository = _unitOfWork.Repository<DistancePricingRule>();


            var user = await userManager.FindByIdAsync(uid);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var service = await serviceRepository.GetEntityByIdAsync(dto.CleaningServiceId);
            if (service == null)
                throw new Exception("Service not found");

            var timeSlot = await timeSlotRepository.GetEntityByIdAsync(dto.TimeSlotId);
            if (timeSlot == null)
                throw new Exception("Time slot not found");

            var address = await addressRepository.GetEntityByIdAsync(dto.AddressId);
            if (address == null)
                throw new Exception("Address not found");

            var bookingAdditionals1 = await _unitOfWork.Repository<AdditionalService>().ListAsync(
                ba => dto.AdditionalServices.Select(a => a.AdditionalServiceId).Contains(ba.Id),
                orderBy: ba => ba.OrderBy(c => c.Id)
            );

            double? distance = await _goongDistanceService.GetDistanceAsync(address.PlaceId, service.PlaceId);
            if (distance == null)
                throw new Exception("Failed to calculate distance");

            var pricingRule = await distancePricingRepository.GetEntityAsync(
                rule => rule.CleaningServiceId == service.Id &&
                        rule.MinDistance <= distance &&
                        rule.MaxDistance >= distance &&
                        rule.IsActive
            );

            if (pricingRule == null)
                throw new Exception("Service is not available for this distance");

            decimal distancePrice = pricingRule.BaseFee;

            var bookingAdditionals = bookingAdditionals1.Select(c => new BookingAdditional
            {
                BookingId = dto.Id,
                Amount = (double)dto.AdditionalServices.FirstOrDefault(a => a.AdditionalServiceId == c.Id).Amount,
                AdditionalServiceId = c.Id,
            }).ToList();

            var totalAdditionalDuration = bookingAdditionals1.Sum(a => a.Duration);
            var booking = new Booking
            {
                Id = dto.Id,
                CustomerId = user.Id,
                CleaningServiceId = service.Id,
                PreferDateStart = dto.BookingDate,
                TimeStart = timeSlot.StartTime,
                TimeEnd = timeSlot.EndTime + TimeSpan.FromMinutes((double)totalAdditionalDuration + 30),
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
                Note = /*dto.Note ??*/ "",
                BookingAdditionals = bookingAdditionals,

            };

            await bookingRepository.AddAsync(booking);
            await _unitOfWork.Complete();

            return booking;
        }

        public async Task<Payment> CreatePayment(Guid bookingId, decimal amount, string paymentMethod)
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

        public async Task<BookingListResponseDto> GetHousekeeperBookingsAsync(ClaimsPrincipal userClaims, int page, int pageSize, string? status)
        {
            var bookingRepository = _unitOfWork.Repository<Booking>();
            var userId = userClaims.FindFirst("id")?.Value;

            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            // Fetch bookings with optional status filtering
            var bookingsQuery = await _unitOfWork.Repository<Booking>().ListAsync(
                filter: c => c.CleaningService.UserId == userId &&
                             c.Status != "IsDeleted" &&
                             (string.IsNullOrEmpty(status) || c.Status == status), // Status filter
                includeProperties: query => query
                    .Include(c => c.CleaningService).ThenInclude(c => c.ServiceImages)
                    .Include(c => c.Payments)
                    .Include(c => c.BookingAdditionals).ThenInclude(c => c.AdditionalService)
                    .Include(c => c.Customer)
            ) ?? new List<Booking>(); // Ensure it's not null

            int totalCount = bookingsQuery.Count();

            var bookings = bookingsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(b => new BookingListItemDto
                {
                    Id = b.Id,
                    PreferDateStart = b.PreferDateStart,
                    TimeStart = b.TimeStart,
                    TimeEnd = b.TimeEnd + TimeSpan.FromMinutes(b.BookingAdditionals.Sum(a => a.AdditionalService.Duration ?? 0)), // Added duration
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
        public async Task<BookingFinishProof> SubmitBookingProofAsync(SubmitBookingProofDTO dto)
        {

            var bookingRepository = _unitOfWork.Repository<Booking>();
            var booking = await bookingRepository.FindAsync(
                b => b.Id == dto.BookingId
            );

            if (booking == null)
            {
                throw new KeyNotFoundException("Booking not found");
            }
            var validStatuses = new[] { "OnGoing", "Paid" };
            if (!validStatuses.Contains(booking.Status))
            {
                throw new InvalidOperationException("Proof can only be submitted for OnGoing or Paid bookings");
            }

            var proof = new BookingFinishProof
            {
                BookingId = dto.BookingId,
                Title = dto.Title,
                ImgUrl = dto.ImgUrl,
            };

            booking.Status = BookingStatus.Completed.ToString();
            booking.CompletedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<BookingFinishProof>().AddAsync(proof);
            bookingRepository.Update(booking); 
            await _unitOfWork.Complete();

            return proof;
        }


    }
}




