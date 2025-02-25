using HCP.Repository.Entities;
using HCP.Service.DTOs.CleaningServiceDTO;
using HCP.Service.DTOs.CustomerDTO;
using HCP.Service.Services.ListService;
using System.Security.Claims;

namespace HCP.Service.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<AppUser?> GetCustomerAsync(ClaimsPrincipal userClaims);
        Task<CustomerProfileDTO?> GetCustomerProfile(ClaimsPrincipal userClaims);
        Task<CustomerProfileDTO> UpdateCustomerProfile(UpdateCusProfileDto customer, ClaimsPrincipal userClaims);
    }
}