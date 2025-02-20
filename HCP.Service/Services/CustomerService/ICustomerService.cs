using HCP.Repository.Entities;
using HCP.Service.DTOs.CustomerDTO;
using System.Security.Claims;

namespace HCP.Service.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<AppUser?> GetCustomerByIdAsync(ClaimsPrincipal userClaims);
        Task<CustomerProfileDTO?> GetCustomerProfileById(ClaimsPrincipal userClaims);
        Task<CustomerProfileDTO> UpdateCustomerProfile(UpdateCusProfileDto customer, ClaimsPrincipal userClaims);
    }
}