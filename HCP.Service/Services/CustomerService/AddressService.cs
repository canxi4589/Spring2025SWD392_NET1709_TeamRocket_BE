using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.CustomerDTO;
using HCP.Service.Services.ListService;
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
                Province = c.Province,
                Title = c.Title,
                ZipCode = c.Zipcode
            }).ToList();
        }
        public async Task<GetAddressListDTO> GetAddressByUserPaging(AppUser user, int? pageIndex, int? pageSize)
        {
            var adrList = _unitOfWork.Repository<Address>().GetAll().Where(c => c.UserId.Equals(user.Id));
            var listAddr = adrList.Select(c => new AddressDTO
            {
                Id = c.Id,
                IsDefault = c.IsDefault,
                Address = c.AddressLine1,
                City = c.City,
                Province = c.Province,
                Title = c.Title,
                ZipCode = c.Zipcode
            });
            if (pageIndex == null || pageSize == null)
            {
                var temp = await PaginatedList<AddressDTO>.CreateAsync(listAddr, 1, listAddr.Count());
                return new GetAddressListDTO
                {
                    Items = temp,
                    hasNext = temp.HasNextPage,
                    hasPrevious = temp.HasPreviousPage,
                    totalCount = listAddr.Count(),
                    totalPages = temp.TotalPages,
                };
            }
            var temp2 = await PaginatedList<AddressDTO>.CreateAsync(listAddr, (int)pageIndex, (int)pageSize);
            return new GetAddressListDTO
            {
                Items = temp2,
                hasNext = temp2.HasNextPage,
                hasPrevious = temp2.HasPreviousPage,
                totalCount = listAddr.Count(),
                totalPages = temp2.TotalPages,
            };
        }
        public async Task<AddressDTO> CreateAddress(AppUser user, CreataAddressDTO addressDTO)
        {

            var addresses = _unitOfWork.Repository<Address>().GetAll().Where(c => c.UserId.Equals(user.Id));
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
                Province = addressDTO.Province,
                Zipcode = addressDTO.Zipcode,
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
                Province = address.Province,
                Title = address.Title,
                ZipCode = address.Zipcode,
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
            address.Province = addressDTO.Province;
            address.Zipcode = addressDTO.Zipcode;
            address.IsDefault = addressDTO.IsDefault;
            address.Title = addressDTO.Title;
            _unitOfWork.Repository<Address>().Update(address);
            await _unitOfWork.Repository<Address>().SaveChangesAsync();
            return new AddressDTO
            {
                Id = address.Id,
                Address = address.AddressLine1,
                City = address.City,
                Province = address.Province,
                Title = address.Title,
                ZipCode = address.Zipcode,
                IsDefault = address.IsDefault
            };
        }
    }
}
