using HCP.Service.DTOs.CustomerDTO;

namespace HCP.Service.Services.CustomerService
{
    public interface IAddressService
    {
        Task<List<AddressDTO>> GetAddressByUser(string id);
    }
}