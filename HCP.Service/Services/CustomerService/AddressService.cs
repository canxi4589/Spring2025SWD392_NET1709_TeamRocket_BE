using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.CustomerDTO;
using Microsoft.AspNetCore.Identity;

namespace HCP.Service.Services.CustomerService
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> userManager;

        public AddressService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        public async Task<List<AddressDTO>> GetAddressByUser(string mail)
        {
            var user = await userManager.FindByEmailAsync(mail);
            if (user != null) 
            {
                var adrList = _unitOfWork.Repository<Address>().GetAll().Where(c => c.UserId.Equals(user.Id));
                return adrList.Select(c => new AddressDTO
                {
                    Address = c.AddressLine1,
                    City = c.City,
                    Province = c.Province,
                    Title = c.Title,
                    ZipCode = c.Zipcode
                }).ToList();
            }
            return null;
        }
    }
}
