using HCP.Repository.Entities;
using HCP.Service.DTOs.CustomerDTO;

namespace HCP.Service.Services.CustomerService
{
    public interface IAddressService
    {
        Task<List<AddressDTO>> GetAddressByUser(AppUser user);
        Task<GetAddressListDTO> GetAddressByUserPaging(AppUser user, int? pageIndex, int? pageSize);
        Task<AddressDTO> CreateAddress(AppUser user, CreataAddressDTO addressDTO);
        Task<AddressDTO> UpdateAddress(AppUser user, UpdateAddressDTO addressDTO);
    }
}