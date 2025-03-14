using HCP.Service.DTOs.BookingDTO;
using HCP.Service.DTOs.CheckoutDTO;
using HCP.Service.DTOs.PaymentDTO;
using System.Security.Claims;

namespace HCP.Service.Services.TemporaryService
{
    public interface ITemporaryStorage
    {
        Task<ConfirmBookingDTO> RetrieveAsync(Guid orderId, ClaimsPrincipal userClaims);
        Task StoreAsync(ConfirmBookingDTO request, ClaimsPrincipal userClaims);
        Task StoreAsync(PaymentBodyDTO request);
        Task StoreTest(ClaimsPrincipal userClaims, Test huh);
        Task<Test> RetrieveTest(ClaimsPrincipal userClaims);
        Task<PaymentBodyDTO> RetrieveAsync(Guid id);
    }
}