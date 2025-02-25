using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.BookingDTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<List<BookingHistoryResponseDTO>> GetBookingByUser(AppUser user)
        {
            var bookingHistoryList = _unitOfWork.Repository<Booking>().GetAll().Where(c => c.Customer == user);
            return bookingHistoryList.Select(c => new BookingHistoryResponseDTO
            {
                BookingId = c.Id,
                PreferDateStart = c.PreferDateStart,
                TimeStart = c.TimeStart,
                TimeEnd = c.TimeEnd,
                Status = c.Status,
                TotalPrice = c.TotalPrice,
                Note = c.Note,
                City = c.City,
                Province = c.Province,
                AddressLine = c.AddressLine,
                ServiceName = c.CleaningService.ServiceName,
                CleaningServiceDuration = c.CleaningService.Duration
            }).ToList();
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
                City = booking.City,
                Province = booking.Province,
                AddressLine = booking.AddressLine,
                ServiceName = booking.CleaningService?.ServiceName ?? "Service Not Available",
                AdditionalServiceName = additionalServiceNames,
                PaymentDate = firstPayment?.PaymentDate ?? DateTime.MinValue,
                PaymentMethod = firstPayment?.PaymentMethod ?? "Unknown",
                PaymentStatus = firstPayment?.Status ?? "Pending",
                CleaningServiceDuration = booking.CleaningService?.Duration ?? 0
            };
        }

    }
}

