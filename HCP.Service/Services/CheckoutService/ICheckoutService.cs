using HCP.Repository.Entities;
using HCP.Service.DTOs.CheckoutDTO;
using System.Security.Claims;

namespace HCP.Service.Services.CheckoutService
{
    public interface ICheckoutService
    {
        Task<CheckoutResponseDTO1> CreateCheckout(CheckoutRequestDTO1 requestDTO, ClaimsPrincipal user);
        Task<bool> ChangeStatusCheckout(Guid checkoutId);
        Task<List<CheckoutResponseDTO1>> GetPendingCheckouts(ClaimsPrincipal user);
    }
}