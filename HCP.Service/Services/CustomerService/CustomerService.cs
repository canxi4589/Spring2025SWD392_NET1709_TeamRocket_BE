using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.CustomerDTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.Services.CustomerService
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;

        public CustomerService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<AppUser?> GetCustomerByIdAsync(ClaimsPrincipal userClaims) 
            => await _userManager.FindByIdAsync(userClaims.FindFirst("id")?.Value);

        public async Task<CustomerProfileDTO?> GetCustomerProfileById(ClaimsPrincipal userClaims)
        {
            var user = await _userManager.FindByIdAsync(userClaims.FindFirst("id")?.Value);
            return new CustomerProfileDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                Birthday = user.Birthday,
                Gender = true,   //must be changed
            };
        }

        //public async Task<AppUser> UpdateCustomerProfile(string id, UpdateCusProfileDto customer)
        //{

        //    var user = await _userManager.FindByIdAsync(id);
        //    if(user == null)
        //    {
        //        throw new Exception("User not found");
        //    }
        //    user.FullName = customer.FullName;
        //    user.Email = customer.Email;
        //    user.PhoneNumber = customer.PhoneNumber;
        //    user.Avatar = customer.Avatar;
        //    user.Birthday = customer.Birthdate;

        //    await _userManager.UpdateAsync(user);
        //    return await _userManager.FindByIdAsync(id);
        //}

        public async Task<CustomerProfileDTO> UpdateCustomerProfile(UpdateCusProfileDto customer, ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirst("id")?.Value;
            var userEmail = userClaims.FindFirst("email")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.FullName = customer.FullName;
            user.PhoneNumber = customer.PhoneNumber;
            user.Avatar = customer.Avatar;
            user.Birthday = customer.Birthdate;
            //user.Gender = customer.Gender;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return new CustomerProfileDTO
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                Birthday = user.Birthday,
                Email = userEmail,
                Gender = true
            };
        }


    }
}
