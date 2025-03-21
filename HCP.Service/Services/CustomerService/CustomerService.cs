﻿using HCP.DTOs.DTOs.CustomerDTO;
using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
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

        public async Task<AppUser?> GetCustomerAsync(ClaimsPrincipal userClaims) 
            => await _userManager.FindByIdAsync(userClaims.FindFirst("id")?.Value);
        
        public async Task<CustomerProfileDTO?> GetCustomerProfile(ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirst("id")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }
            var user = await _userManager.FindByIdAsync(userId);
            return user == null
                ? throw new KeyNotFoundException("User not found")
                : new CustomerProfileDTO
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Birthday = user.Birthday,
                Gender = user.Gender,
                Avatar = user.Avatar
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

            var user = await _userManager.FindByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
            user.FullName = customer.FullName;
            user.PhoneNumber = customer.PhoneNumber;
            user.Birthday = customer.Birthdate;
            user.Gender = customer.Gender;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return new CustomerProfileDTO
            {
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Birthday = user.Birthday,
                Email = userEmail,
                Gender = user.Gender,
                Avatar = user.Avatar
            };
        }
        public async Task<string> UpdateCustomerAvatarProfile(string avatar, ClaimsPrincipal userClaims)
        {
            var userId = userClaims.FindFirst("id")?.Value;
            var userEmail = userClaims.FindFirst("email")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var user = await _userManager.FindByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
            user.Avatar = avatar;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return user.Avatar;
        }

        public async Task<CustomerCheckoutProfile?> GetCustomerCheckoutProfile(ClaimsPrincipal userClaims)
        {
            var user = await _userManager.FindByIdAsync(userClaims.FindFirst("id")?.Value);
            var adrList = _unitOfWork.Repository<Address>().GetAll().Where(c => c.UserId.Equals(user.Id));
            var result = new CustomerCheckoutProfile
            {
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Birthday = user.Birthday,
            }
            ;
            result.addressDTOs = adrList.Select(c => new AddressDTO
            {
                Id = c.Id,
                IsDefault = c.IsDefault,
                Address = c.AddressLine1,
                City = c.City,
                District = c.District,
                Title = c.Title,
                PlaceId = c.PlaceId
            }).ToList();
           return result;
        }

    }
}
