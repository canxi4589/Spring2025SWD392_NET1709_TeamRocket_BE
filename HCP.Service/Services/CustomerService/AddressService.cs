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

        public async Task<List<AddressDTO>> GetAddressByUser(AppUser user)
        {
            var adrList = _unitOfWork.Repository<Address>().GetAll().Where(c => c.UserId.Equals(user.Id));
            return adrList.Select(c => new AddressDTO
            {
                Id = c.Id,
                IsDefault = c.IsDefault,
                Address = c.AddressLine1,
                City = c.City,
                District = c.District,
                Title = c.Title,
                PlaceId = c.PlaceId
            }).ToList();
        }
        public async Task<AddressDTO> CreateAddress(AppUser user, CreataAddressDTO addressDTO)
        {

            var addresses = _unitOfWork.Repository<Address>().GetAll().Where(c => c.UserId.Equals(user.Id));
            if (addresses.Count() > 9)
            {
                throw new Exception("You can only have 10 addresses at the same time");
            }
            if (addressDTO.IsDefault)
            {
                foreach (var adr in addresses)
                {
                    adr.IsDefault = false;
                    _unitOfWork.Repository<Address>().Update(adr);
                }
            }
            var address = new Address
            {
                AddressLine1 = addressDTO.Address,
                City = addressDTO.City,
                District = addressDTO.District,
                PlaceId = addressDTO.PlaceId,
                IsDefault = addressDTO.IsDefault,
                Title = addressDTO.Title,
                UserId = user.Id,
                User = user
            };
            await _unitOfWork.Repository<Address>().AddAsync(address);
            await _unitOfWork.Repository<Address>().SaveChangesAsync();
            return new AddressDTO
            {
                Id = address.Id,
                Address = address.AddressLine1,
                City = address.City,
                District = addressDTO.District,
                Title = address.Title,
                PlaceId = addressDTO.PlaceId,
                IsDefault = address.IsDefault
            };
        }
        public async Task<AddressDTO> UpdateAddress(AppUser user, UpdateAddressDTO addressDTO)
        {
            var address = await _unitOfWork.Repository<Address>().FindAsync(c => c.Id.Equals(addressDTO.Id));
            if (address == null)
            {
                throw new Exception("Address not found");
            }
            if (addressDTO.IsDefault)
            {
                var addresses = _unitOfWork.Repository<Address>().GetAll().Where(c => c.UserId.Equals(user.Id));
                foreach (var adr in addresses)
                {
                    adr.IsDefault = false;
                    _unitOfWork.Repository<Address>().Update(adr);
                }
            }
            address.AddressLine1 = addressDTO.Address;
            address.City = addressDTO.City;
            address.District = addressDTO.District;
            address.PlaceId = addressDTO.PlaceId;
            address.IsDefault = addressDTO.IsDefault;
            address.Title = addressDTO.Title;
            _unitOfWork.Repository<Address>().Update(address);
            await _unitOfWork.Repository<Address>().SaveChangesAsync();
            return new AddressDTO
            {
                Id = address.Id,
                Address = address.AddressLine1,
                City = address.City,
                District = address.District,
                Title = address.Title,
                PlaceId = address.PlaceId,
                IsDefault = address.IsDefault
            };
        }
    }
}
