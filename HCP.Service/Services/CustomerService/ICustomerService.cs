using HCP.Repository.Entities;
using HCP.Service.DTOs.CustomerDTO;

namespace HCP.Service.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<AppUser?> GetCustomerByIdAsync(string id);
        Task<CustomerProfileDTO?> GetCustomerProfileById(string id);
        Task<AppUser> UpdateCustomerProfile(string id, UpdateCusProfileDto customer);
    }
}