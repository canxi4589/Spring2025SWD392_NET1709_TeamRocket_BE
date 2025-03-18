using HCP.DTOs.DTOs.HousekeeperDTOs;
using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using Humanizer;
using Microsoft.AspNetCore.Identity;
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
    }
}
