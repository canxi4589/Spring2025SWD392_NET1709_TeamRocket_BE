﻿using HCP.Repository.Entities;
using HCP.Repository.GenericRepository;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.BookingDTO;
using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.Services.ListService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
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
        public async Task<BookingHistoryResponseListDTO> GetBookingByUser(AppUser user, int? pageIndex, int? pageSize, string? status, int? day, int? month, int? year)
        {
            var bookingHistoryList = _unitOfWork.Repository<Booking>().GetAll().Where(c => c.Customer == user).Include(c => c.CleaningService).OrderByDescending(c => c.PreferDateStart);
            if (status != null)
            {
                if (status.Equals("Recently")) bookingHistoryList.OrderByDescending(c => c.CreatedDate);
                if (status.Equals("On-going"))
                {
                    bookingHistoryList = (IOrderedQueryable<Booking>)bookingHistoryList.Where(c => c.Status.Equals("On-going"));
                }
            }
            if (day.HasValue && month.HasValue && year.HasValue)
            {
                var targetDate = new DateTime(year.Value, month.Value, day.Value);
                bookingHistoryList = (IOrderedQueryable<Booking>)bookingHistoryList.Where(c => c.PreferDateStart.Day == targetDate.Day && c.PreferDateStart.Month == targetDate.Month && c.PreferDateStart.Year == targetDate.Year);
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
                Location = c.AddressLine + ", " + c.Province + ", " + c.City,
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
                Location = booking.AddressLine + " " + booking.Province + " " + booking.City,
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
    }
}

