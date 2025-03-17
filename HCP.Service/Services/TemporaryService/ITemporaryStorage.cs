using HCP.DTOs.DTOs.BookingDTO;
using HCP.DTOs.DTOs.PaymentDTO;
using HCP.DTOs.DTOs.WalletDTO;
using System.Security.Claims;

namespace HCP.Service.Services.TemporaryService
{
    public interface ITemporaryStorage
    {
        Task<ConfirmBookingDTO> RetrieveAsync(Guid orderId, ClaimsPrincipal userClaims);
        Task StoreAsync(ConfirmBookingDTO request, ClaimsPrincipal userClaims);
        Task StoreAsync(PaymentBodyDTO request);
        Task StoreTest(ClaimsPrincipal userClaims, Test huh);
        Task StoreAsync(WalletDepositRequestDTO request);
        Task<WalletDepositRequestDTO> RetrieveDepositAsync(Guid id);
        Task<Test> RetrieveTest(ClaimsPrincipal userClaims);
        Task<PaymentBodyDTO> RetrieveAsync(Guid id);
    }
}