using System.Security.Claims;
using HCP.Repository.Entities;
using HCP.Service.DTOs.WalletDTO;

namespace HCP.Service.Services.WalletService
{
    public interface IWalletService
    {
        Task<GetWalletWithdrawRequestListDTO> GetTransacts(AppUser user, int? pageIndex, int? pageSize, string searchField, string? fullname, string? phonenumber, string? mail);
        Task<double> getUserBalance(AppUser user);
        Task<double> VNDMoneyExchangeFromUSD(decimal amount);
        Task<WalletWithdrawRequestDTO> CreateWithdrawRequest(decimal amount, AppUser user);
        Task<WalletTransactionDepositResponseDTO> createDepositTransaction(decimal amount, AppUser user);
        Task<WalletTransactionDepositResponseDTO> processDepositTransaction(Guid depoTrans, bool successOrNot);
        Task<WalletTransactionWithdrawResponseDTO> StaffProccessWithdraw(Guid transId, bool action);
    }
}