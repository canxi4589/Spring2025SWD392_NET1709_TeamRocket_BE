using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.CustomerDTO;
using HCP.Service.DTOs.WalletDTO;
using HCP.Service.Services.CustomerService;
using HCP.Service.Services.ListService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HCP.Service.Services.WalletService
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICustomerService _customerService;

        public WalletService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, ICustomerService customerService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _customerService = customerService;
        }
        public async Task<WalletWithdrawRequestDTO> CreateWithdrawRequest(decimal amount, AppUser user)
        {
            if (user.BalanceWallet < (Double)amount) throw new Exception("Your balance is lower than the amount you wish to withdraw!");
            var wTransaction = new WalletTransaction
            {
                AfterAmount = 0,
                Current = 0,
                User = user,
                UserId = user.Id,
                Amount = amount,
                Type = "WithdrawRequest"
            };
            await _unitOfWork.Repository<WalletTransaction>().AddAsync(wTransaction);
            await _unitOfWork.Repository<WalletTransaction>().SaveChangesAsync();
            return new WalletWithdrawRequestDTO
            {
                Amount = amount,
                Type = "WithdrawRequest",
                UserId = user.Id,
                FullName = user.FullName,
                Mail = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }
        public async Task<GetWalletWithdrawRequestListDTO> GetTransacts(AppUser user, int? pageIndex, int? pageSize, string searchField, string? fullname, string? phonenumber, string? mail)
        {
            var wTransactionList = _unitOfWork.Repository<WalletTransaction>().GetAll();
            if (searchField.Equals("WithdrawRequestUser") || searchField.Equals("WithdrawUser") || searchField.Equals("WithdrawRejectUser") || searchField.Equals("ShowHistoryUser"))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList.Where(c => c.User.Id == user.Id);
            }
            if (searchField.Equals("WithdrawRequestStaff") || searchField.Equals("WithdrawRequestUser"))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList.Where(c => c.Type.Equals("WithdrawRequest"));
            }
            if (searchField.Equals("WithdrawStaff") || searchField.Equals("WithdrawUser"))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList.Where(c => c.Type.Equals("Withdraw"));
            }
            if (searchField.Equals("WithdrawRejectStaff") || searchField.Equals("WithdrawRejectUser"))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList.Where(c => c.Type.Equals("WithdrawReject"));
            }
            if (searchField.Equals("ShowHistoryUser"))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList.Where(c => c.Type.Equals("Withdraw") || c.Type.Equals("WithdrawReject") || c.Type.Equals("WithdrawRequest"));
            }
            if (searchField.Equals("ShowHistoryStaff"))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList.Where(c => c.Type.Equals("Withdraw") || c.Type.Equals("WithdrawReject"));
            }
            if (!string.IsNullOrEmpty(fullname))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList.Where(c => c.User.FullName.Equals(fullname));
            }
            if (!string.IsNullOrEmpty(phonenumber))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList.Where(c => c.User.PhoneNumber.Equals(phonenumber));
            }
            if (!string.IsNullOrEmpty(mail))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList.Where(c => c.User.Email.Equals(mail));
            }
            var listTrans = wTransactionList.Select(c => new WalletWithdrawStaffShowDTO
            {
                Id = c.Id,
                Amount = c.Amount,
                Type = c.Type,
                FullName = c.User.FullName,
                Mail = c.User.Email,
                PhoneNumber = c.User.PhoneNumber,
                UserId = c.UserId,
            });
            if (pageIndex == null || pageSize == null)
            {
                var temp = await PaginatedList<WalletWithdrawStaffShowDTO>.CreateAsync(listTrans, 1, listTrans.Count());
                return new GetWalletWithdrawRequestListDTO
                {
                    Items = temp,
                    hasNext = temp.HasNextPage,
                    hasPrevious = temp.HasPreviousPage,
                    totalCount = listTrans.Count(),
                    totalPages = temp.TotalPages,
                };
            }
            var temp2 = await PaginatedList<WalletWithdrawStaffShowDTO>.CreateAsync(listTrans, (int)pageIndex, (int)pageSize);
            return new GetWalletWithdrawRequestListDTO
            {
                Items = temp2,
                hasNext = temp2.HasNextPage,
                hasPrevious = temp2.HasPreviousPage,
                totalCount = listTrans.Count(),
                totalPages = temp2.TotalPages,
            };
        }
        public async Task<WalletTransactionWithdrawResponseDTO> StaffProccessWithdraw(Guid transId, bool action)
        {
            var transact = await _unitOfWork.Repository<WalletTransaction>().FindAsync(c => c.Id.Equals(transId));
            if (transact == null) throw new Exception("Transaction not found");
            var user = await _userManager.FindByIdAsync(transact.UserId);
            if (user.Id != transact.UserId)
            {
                throw new Exception("There something wrong in creating transaction process!");
            }
            if (action)
            {
                //Approve
                transact.Type = "Withdraw";
                transact.AfterAmount = (Decimal)user.BalanceWallet - transact.Amount;
                user.BalanceWallet -= (Double)transact.Amount;
                transact.Current = (Decimal)user.BalanceWallet;
                _unitOfWork.Repository<WalletTransaction>().Update(transact);
                await _userManager.UpdateAsync(user);
                await _unitOfWork.Repository<WalletTransaction>().SaveChangesAsync();
                return new WalletTransactionWithdrawResponseDTO
                {
                    Amount = (Double)transact.Amount,
                    Current = user.BalanceWallet,
                    AfterAmount = (Double)transact.AfterAmount,
                    Type = "Withdraw",
                    UserId = user.Id,
                    FullName = user.FullName,
                    Mail = user.Email,
                    PhoneNumber = user.PhoneNumber,
                };
            }
            else
            {
                //Reject
                transact.Type = "WithdrawReject";
                transact.AfterAmount = (Decimal)user.BalanceWallet;
                transact.Current = (Decimal)user.BalanceWallet;
                _unitOfWork.Repository<WalletTransaction>().Update(transact);
                await _unitOfWork.Repository<WalletTransaction>().SaveChangesAsync();
                return new WalletTransactionWithdrawResponseDTO
                {
                    Amount = (Double)transact.Amount,
                    Current = user.BalanceWallet,
                    AfterAmount = (Double)transact.AfterAmount,
                    Type = "WithdrawReject",
                    UserId = user.Id,
                    FullName = user.FullName,
                    Mail = user.Email,
                };
            }
        }
        public async Task<WalletTransactionDepositResponseDTO> processDepositTransaction(string status, decimal amount, AppUser user)
        {
            if (status != "00") throw new Exception("Transaction failed");
            var wTransaction = new WalletTransaction
            {
                AfterAmount = (Decimal)user.BalanceWallet + amount,
                Current = (Decimal)user.BalanceWallet,
                User = user,
                UserId = user.Id,
                Amount = amount,
                Type = "Deposit"
            };
            user.BalanceWallet += (Double)amount;
            await _userManager.UpdateAsync(user);
            await _unitOfWork.Repository<WalletTransaction>().AddAsync(wTransaction);
            await _unitOfWork.Repository<WalletTransaction>().SaveChangesAsync();
            return new WalletTransactionDepositResponseDTO
            {
                Amount = (Double)amount,
                Current = user.BalanceWallet,
                AfterAmount = (Double)wTransaction.AfterAmount,
                Type = "Deposit",
                UserId = user.Id,
                FullName = user.FullName,
                Mail = user.Email,
                PhoneNumber = user.PhoneNumber,
                Description = "Deposit success"
            };
        }
    }
}

