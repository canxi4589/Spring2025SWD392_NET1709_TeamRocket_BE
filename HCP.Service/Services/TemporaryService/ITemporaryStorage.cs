using HCP.Service.DTOs.BookingDTO;
using System.Security.Claims;

namespace HCP.Service.Services.TemporaryService
{
    public interface ITemporaryStorage
    {
        Task<ConfirmBookingDTO> RetrieveAsync(Guid orderId, ClaimsPrincipal userClaims);
        Task StoreAsync(ConfirmBookingDTO request, ClaimsPrincipal userClaims);
        Task StoreTest(ClaimsPrincipal userClaims, Test huh);
        Task<Test> RetrieveTest(ClaimsPrincipal userClaims);
    }
}