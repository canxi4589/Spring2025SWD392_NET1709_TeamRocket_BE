using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.BookingDTO;
using HCP.Service.Services.ListService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HCP.Service.Services.BookingService
{
    public class BookingTransactionService : IBookingTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IBookingService bookingService;

        public BookingTransactionService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IBookingService bookingService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            this.bookingService = bookingService;
        }

        public async Task<BookingTransactionShowListDTO> GetBookingTransactionHistory(AppUser user, bool customerOrNot, int? pageIndex, int? pageSize, int? day, int? month, int? year, string? status)
        {
            var paymentList = _unitOfWork.Repository<Payment>().GetAll();
            if (customerOrNot)
            {
                paymentList = (IOrderedQueryable<Payment>)paymentList.Where(x => x.Booking.CustomerId == user.Id);
            }
            else
            {
                paymentList = (IOrderedQueryable<Payment>)paymentList.Where(x => x.Booking.CleaningService.UserId == user.Id);
            }
            if (day.HasValue && month.HasValue && year.HasValue)
            {
                var targetDate = new DateTime(year.Value, month.Value, day.Value);
                paymentList = (IOrderedQueryable<Payment>)paymentList.Where(c => c.PaymentDate == targetDate);
            }
            if (status != null)
            {
                paymentList = (IOrderedQueryable<Payment>)paymentList.Where(c => c.Status == status);
            }
            var listPayments = paymentList.Select(c => new BookingTransactionShowDTO
            {
                Ammount = c.Amount,
                BookingId = c.BookingId,
                Commission = 0,
                CustomerMail = c.Booking.Customer.Email,
                CustomerName = c.Booking.Customer.FullName,
                CustomerPhoneNumber = c.Booking.Customer.PhoneNumber,
                PaymentDate = c.PaymentDate,
                PaymentId = c.Id,
                PaymentMethod = c.PaymentMethod,
                Status = c.Status,
                Type = c.Status,
                ServiceName = c.Booking.CleaningService.ServiceName,
                LinkUrl = c.Booking.CleaningService.ServiceImages.FirstOrDefault().LinkUrl
            });
            if (pageIndex == null || pageSize == null)
            {
                var temp = await PaginatedList<BookingTransactionShowDTO>.CreateAsync(listPayments, 1, listPayments.Count());
                return new BookingTransactionShowListDTO
                {
                    Items = temp,
                    hasNext = temp.HasNextPage,
                    hasPrevious = temp.HasPreviousPage,
                    totalCount = listPayments.Count(),
                    totalPages = temp.TotalPages,
                };
            }
            var temp2 = await PaginatedList<BookingTransactionShowDTO>.CreateAsync(listPayments, (int)pageIndex, (int)pageSize);
            return new BookingTransactionShowListDTO
            {
                Items = temp2,
                hasNext = temp2.HasNextPage,
                hasPrevious = temp2.HasPreviousPage,
                totalCount = listPayments.Count(),
                totalPages = temp2.TotalPages,
            };
        }
        public async Task<BookingTransactionDetailShowDTO> GetBookingTransactionHistoryDetail(Payment payment)
        {
            var bookings = await _unitOfWork.Repository<Booking>().ListAsync(
                filter: c => c.Id == payment.BookingId,
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

            var bookingone = await bookingService.GetBookingDetailById(booking);
            var serviceImages = booking.CleaningService?.ServiceImages;

            return new BookingTransactionDetailShowDTO
            {
                Ammount = payment.Amount,
                BookingId = payment.BookingId,
                Commission = 0,
                HousekeeperName = bookingone.HousekeeperName,
                HousekeeperMail = bookingone.HouseKeeperMail,
                HousekeeperPhoneNumber = bookingone.HouseKeeperPhoneNumber,
                CustomerName = bookingone.CustomerName,
                CustomerMail = bookingone.CustomerMail,
                CustomerPhoneNumber = bookingone.CustomerPhoneNumber,
                PaymentDate = payment.PaymentDate,
                PaymentId = payment.Id,
                PaymentMethod = payment.PaymentMethod,
                Status = payment.Status,
                Type = payment.Status,
                ServiceName = bookingone.ServiceName,
                LinkUrl = serviceImages?.FirstOrDefault()?.LinkUrl ?? "ImgLink not found"
            };
        }
    }
}
