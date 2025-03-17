using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs;
using HCP.Service.DTOs.HousekeeperDTOs;
using HCP.Service.Services.EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.Services.AuthenticationService
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        public string _frontendurl;
        private readonly IEmailSenderService _emailSenderService;

        public AuthenticationService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IConfiguration configuration, IEmailSenderService emailSenderService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _configuration = configuration;
            _frontendurl = configuration["Url:Frontend"] ?? "";
            _emailSenderService = emailSenderService;
        }

        public async Task<HousekeeperRegisterResponseDTO?> HousekeeperRegister(HousekeeperRegisterRequestDTO requestDTO)
        {
            if (await _userManager.FindByEmailAsync(requestDTO.Email) != null)
            {
                throw new InvalidOperationException(AuthenticationConst.EmailTaken);
            }

            if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == requestDTO.PhoneNumber))
            {
                throw new InvalidOperationException(AuthenticationConst.PhoneTaken);
            }

            var housekeeper = new AppUser
            {
                UserName = requestDTO.Email,
                Email = requestDTO.Email,
                PhoneNumber = requestDTO.PhoneNumber,
                FullName = requestDTO.FullName,
                Avatar = requestDTO.Avatar,
                PDF = requestDTO.Pdf,
                IdCardFront = requestDTO.IdCardFront,
                IdCardBack = requestDTO.IdCardBack,
                IsHousekeeperVerified = false
            };

            var result = await _userManager.CreateAsync(housekeeper, requestDTO.Password);

            if (!result.Succeeded)
            {
                var identityErrors = result.Errors.Select(e => e.Description).ToList();
                throw new Exception(string.Join(", ", identityErrors));
            }

            await _userManager.AddToRoleAsync(housekeeper, KeyConst.Housekeeper);

            var housekeeperSkill = requestDTO.HousekeeperCategories.Select(categoryId => new HousekeeperSkill
            {
                HousekeeperId = housekeeper.Id,
                CategoryId = categoryId, 
                Status = string.Empty, 
                SkillLevel = 1 
            }).ToList();

            await _unitOfWork.Repository<HousekeeperSkill>().AddRangeAsync(housekeeperSkill);
            await _unitOfWork.Repository<HousekeeperSkill>().SaveChangesAsync();

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(housekeeper);
            var confirmationLink = $"{_frontendurl}/confirm-email?userId={housekeeper.Id}&token={Uri.EscapeDataString(token)}";

            var emailBody = EmailBodyTemplate.GetRegistrationConfirmationEmail("https://picsum.photos/300/500", housekeeper.Email, confirmationLink);
            _emailSenderService.SendEmail(housekeeper.Email, "Confirm your Housekeeper Account", emailBody);

            return new HousekeeperRegisterResponseDTO
            {
                HousekeeperId = housekeeper.Id,
                FullName = housekeeper.FullName,
                Email = housekeeper.Email,
                PhoneNumber = housekeeper.PhoneNumber,
                Avatar = housekeeper.Avatar,
                Pdf = housekeeper.PDF,
                IdCardFront = housekeeper.IdCardFront,
                IdCardBack = housekeeper.IdCardBack,
                HousekeeperCategories = requestDTO.HousekeeperCategories
            };
        }
    }
}
