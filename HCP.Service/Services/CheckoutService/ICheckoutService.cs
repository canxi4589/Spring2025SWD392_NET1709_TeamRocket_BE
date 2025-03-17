using HCP.DTOs.DTOs.CheckoutDTO;
using HCP.Repository.Entities;
using System.Security.Claims;

namespace HCP.Service.Services.CheckoutService
{
    public interface ICheckoutService
    {
        Task<CheckoutResponseDTO1> CreateCheckout(CheckoutRequestDTO1 requestDTO, ClaimsPrincipal user);
        Task<bool> ChangeStatusCheckout(Guid checkoutId);
        Task<List<CheckoutResponseDTO1>> GetPendingCheckouts(ClaimsPrincipal user);

        Task<CheckoutResponseDTO1> GetCheckoutById(Guid checkoutId);
    }
}