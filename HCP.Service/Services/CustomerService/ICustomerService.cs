using HCP.DTOs.DTOs.CustomerDTO;
using HCP.Repository.Entities;
using System.Security.Claims;

namespace HCP.Service.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<AppUser?> GetCustomerAsync(ClaimsPrincipal userClaims);
        Task<CustomerProfileDTO?> GetCustomerProfile(ClaimsPrincipal userClaims);
        Task<CustomerProfileDTO> UpdateCustomerProfile(UpdateCusProfileDto customer, ClaimsPrincipal userClaims);
        Task<string> UpdateCustomerAvatarProfile(string avatar, ClaimsPrincipal userClaims);
        Task<CustomerCheckoutProfile?> GetCustomerCheckoutProfile(ClaimsPrincipal userClaims);
    }
}