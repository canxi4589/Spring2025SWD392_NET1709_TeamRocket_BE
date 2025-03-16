using System.Security.Claims;
using HCP.Repository.Entities;
using HCP.Service.DTOs.BookingDTO;
using HCP.Service.DTOs.WalletDTO;

namespace HCP.Service.Services.WalletService
{
    public interface IWalletService
    {
        Task<GetWalletWithdrawRequestListDTO> GetTransacts(AppUser user, int? pageIndex, int? pageSize, string searchField, string? fullname, string? phonenumber, string? mail);
        Task<double> getUserBalance(AppUser user);
        Task<double> getUserBalance(ClaimsPrincipal user);

        Task<double> VNDMoneyExchangeFromUSD(decimal amount);
        Task<RefundRequestDTO> CreateRefundRequest(Guid bookingId, string ProofOfPayment, string Reason, AppUser user);
        Task<RefundRequestShowListDTO> GetRefundRequestsAsync(string? search, int? pageIndex, int? pageSize, string? status);
        Task<RefundRequestShowDetailDTO> GetRefundRequestByIdAsync(Guid refundRequestId);
        Task<RefundRequestDTO> StaffProccessRefund(Guid refundId, bool action, ClaimsPrincipal claims);
        Task<WalletWithdrawRequestDTO> CreateWithdrawRequest(decimal amount, AppUser user);
        Task<WalletTransactionDepositResponseDTO> createDepositTransaction(decimal amount, AppUser user);
        Task<WalletTransactionDepositResponseDTO> processDepositTransaction(Guid depoTrans, bool successOrNot);
        Task<WalletTransactionWithdrawResponseDTO> StaffProccessWithdraw(Guid transId, bool action);
        Task DeduceFromWallet(ClaimsPrincipal user, decimal amount);
        Task<RevenueHousekeeperDatasListShowDTO> GetRevenueHousekeeperDatas(AppUser user, bool dayRevenue, bool weekRevenue, bool yearRevenue, bool yearsRevenue, int? dayStart, int? monthStart, int? yearStart, int? dayEnd, int? monthEnd, int? yearEnd);
    }
}