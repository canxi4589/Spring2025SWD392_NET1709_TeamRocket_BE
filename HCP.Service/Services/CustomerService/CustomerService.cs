using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.CustomerDTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        public async Task<AppUser?> GetCustomerByIdAsync(string id) => await _userManager.FindByIdAsync(id);

        public async Task<CustomerProfileDTO?> GetCustomerProfileById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return new CustomerProfileDTO
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                Birthday = user.Birthday,
                Gender = true,   //must be changed
            };
        }

        public async Task<AppUser> UpdateCustomerProfile(string id, UpdateCusProfileDto customer)
        {

            var user = await _userManager.FindByIdAsync(id);
            if(user == null)
            {
                throw new Exception("User not found");
            }
            user.FullName = customer.FullName;
            user.Email = customer.Email;
            user.PhoneNumber = customer.PhoneNumber;
            user.Avatar = customer.Avatar;
            user.Birthday = customer.Birthdate;

            await _userManager.UpdateAsync(user);
            return await _userManager.FindByIdAsync(id);
        }


    }
}
