using HCP.DTOs.DTOs.HousekeeperDTOs;
using HCP.DTOs.DTOs.WalletDTO;
using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.Interfaces;
using HCP.Service.Services.ListService;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.Services.HousekeeperService
{
    public class HousekeeperService : IHousekeeperService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public HousekeeperService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<HousekeeperProfileDTO> GetHousekeeperProfile(ClaimsPrincipal currentHousekeeper)
        {
            var housekeeperId = currentHousekeeper.FindFirst("id")?.Value ?? throw new KeyNotFoundException(CommonConst.HousekeeperNotFound);
            var housekeeper = await _userManager.FindByIdAsync(housekeeperId) ?? throw new KeyNotFoundException(CommonConst.HousekeeperNotFound);
            var housekeeperAddress = await _unitOfWork
                .Repository<Address>()
                .FindAsync(c => c.UserId.Equals(housekeeperId)) ?? throw new KeyNotFoundException(CommonConst.HousekeeperDefaultAddress);
            var housekeeperCategories = _unitOfWork.Repository<HousekeeperSkill>().GetAll().Where(c => c.HousekeeperId == housekeeperId).Select(c => c.CategoryId).ToList();

            return new HousekeeperProfileDTO
            {
                AddressLine1 = housekeeperAddress.AddressLine1,
                ApprovedBy = housekeeper.HousekeeperVerifiedBy ?? string.Empty,
                Avatar = housekeeper.Avatar,
                City = housekeeperAddress.City,
                District = housekeeperAddress.District,
                Email = housekeeper.Email,
                FullName = housekeeper.FullName,
                HousekeeperCategories = housekeeperCategories,
                HousekeeperId = housekeeperId,
                IdCardBack = housekeeper.IdCardBack,
                IdCardFront = housekeeper.IdCardFront,
                Pdf = housekeeper.PDF,
                PhoneNumber = housekeeper.PhoneNumber,
                PlaceId = housekeeperAddress.PlaceId,
                Status = housekeeper.HousekeeperStatus,
                Title = housekeeperAddress.Title
            };
        }

        public async Task<HousekeeperEarningListDTO> GetHousekeeperEarnings(ClaimsPrincipal housekeeperClaim, int? pageIndex, int? pageSize, int? day, int? month, int? year)
        {
            var housekeeperId = housekeeperClaim.FindFirst("id")?.Value ?? throw new KeyNotFoundException(CommonConst.HousekeeperNotFound);
            var housekeeper = await _userManager.FindByIdAsync(housekeeperId) ?? throw new KeyNotFoundException(CommonConst.HousekeeperNotFound);
            var bookingRepository = _unitOfWork.Repository<Booking>().GetAll()
                .Include(c => c.CleaningService).Where(s => (s.CleaningService.UserId == housekeeper.Id) && (s.Status.Equals(BookingStatus.Completed.ToString()) || s.Status.Equals(BookingStatus.Refunded.ToString())))
                .OrderByDescending(b => b.CompletedAt);
            if (day.HasValue && month.HasValue && year.HasValue)
            {
                //var targetDay = new DateTime(day.Value, year.Value, month.
                bookingRepository = (IOrderedQueryable<Booking>)bookingRepository
                    .Where(c => c.CompletedAt.Value.Day == day && c.CompletedAt.Value.Month == month && c.CompletedAt.Value.Year == year);
            }
            var earningList = bookingRepository.Select(b => new HousekeeperEarningDTO
            {
                Price = b.TotalPrice,
                Fee = (decimal)b.Fee,
                YourEarn = b.TotalPrice * 0.9m,
                Date = b.CompletedAt ?? b.CreatedDate,
                BookingId = b.Id,
                Status = b.Status
            });
            if (pageIndex == null || pageSize == null)
            {
                var temp = await PaginatedList<HousekeeperEarningDTO>.CreateAsync(earningList, 1, earningList.Count());
                return new HousekeeperEarningListDTO
                {
                    Items = temp,
                    hasNext = temp.HasNextPage,
                    hasPrevious = temp.HasPreviousPage,
                    totalCount = earningList.Count(),
                    totalPages = temp.TotalPages,
                };
            }
            var temp2 = await PaginatedList<HousekeeperEarningDTO>.CreateAsync(earningList, (int)pageIndex, (int)pageSize);
            return new HousekeeperEarningListDTO
            {
                Items = temp2,
                hasNext = temp2.HasNextPage,
                hasPrevious = temp2.HasPreviousPage,
                totalCount = earningList.Count(),
                totalPages = temp2.TotalPages,
            };
        }
    }
}
