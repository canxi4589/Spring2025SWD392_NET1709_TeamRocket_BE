using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Azure;
using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.CustomerDTO;
using HCP.Service.DTOs.WalletDTO;
using HCP.Service.Integrations.Currency;
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
            double expectedWithdraw = 0;
            double availableWithdraw = 0;
            var pendingTransact = _unitOfWork.Repository<WalletTransaction>().GetAll().Where(c=>(c.User == user && c.Status.Equals(TransactionStatus.Pending.ToString())));
            foreach (var transaction in pendingTransact) 
            {
                expectedWithdraw += (double)transaction.Amount;
            }
            if (user.BalanceWallet<(expectedWithdraw+(double)amount))
            {
                availableWithdraw = user.BalanceWallet-(expectedWithdraw);
                throw new Exception($"You're having pending withdraw requests that total more than your balance! @The amount reccomended is {availableWithdraw}");
            }
            if (user.BalanceWallet < (double)amount) throw new Exception("Your balance is lower than the amount you want to withdraw!");
            var wTransaction = new WalletTransaction
            {
                AfterAmount = 0,
                Current = 0,
                User = user,
                UserId = user.Id,
                Amount = amount,
                Type = "Withdraw",
                Status = TransactionStatus.Pending.ToString(),
                CreatedDate = DateTime.UtcNow,
            };
            await _unitOfWork.Repository<WalletTransaction>().AddAsync(wTransaction);
            await _unitOfWork.Repository<WalletTransaction>().SaveChangesAsync();
            return new WalletWithdrawRequestDTO
            {
                Amount = amount,
                Type = wTransaction.Type,
                UserId = user.Id,
                FullName = user.FullName,
                Mail = user.Email,
                PhoneNumber = user.PhoneNumber,
                CreatedDate = wTransaction.CreatedDate,
                Status = wTransaction.Status
            };
        }
        public async Task<GetWalletWithdrawRequestListDTO> GetTransacts(AppUser user, int? pageIndex, int? pageSize, string searchField, string? fullname, string? phonenumber, string? mail)
        {
            var wTransactionList = _unitOfWork.Repository<WalletTransaction>().GetAll().OrderByDescending(c=>c.CreatedDate);
            if (searchField.Equals(TransactionType.WithdrawRequestUser.ToString()) || searchField.Equals(TransactionType.WithdrawUser.ToString()) 
                || searchField.Equals(TransactionType.WithdrawRejectUser.ToString()) || searchField.Equals(TransactionType.ShowAllHistoryUser.ToString()) 
                || searchField.Equals(TransactionType.ShowWithdrawHistoryUser.ToString()) || searchField.Equals(TransactionType.Deposit.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList.Where(c => c.User.Id == user.Id);
            }

            if (searchField.Equals(TransactionType.WithdrawRequestStaff.ToString()) || searchField.Equals(TransactionType.WithdrawRequestUser.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.Type.Equals(TransactionType.Withdraw.ToString()) && c.Status.Equals(TransactionStatus.Pending.ToString()));
            }

            if (searchField.Equals(TransactionType.Deposit.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.Type.Equals(TransactionType.Deposit.ToString()));
            }

            if (searchField.Equals(TransactionType.WithdrawStaff.ToString()) || searchField.Equals(TransactionType.WithdrawUser.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.Type.Equals(TransactionType.Withdraw.ToString()) && c.Status.Equals(TransactionStatus.Done.ToString()));
            }

            if (searchField.Equals(TransactionType.WithdrawRejectStaff.ToString()) || searchField.Equals(TransactionType.WithdrawRejectUser.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.Type.Equals(TransactionType.Withdraw.ToString()) && c.Status.Equals(TransactionStatus.Fail.ToString()));
            }

            if (searchField.Equals(TransactionType.ShowWithdrawHistoryUser.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.Type.Equals(TransactionType.Withdraw.ToString()));
            }

            if (searchField.Equals(TransactionType.ShowAllHistoryUser.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => (c.Type.Equals(TransactionType.Withdraw.ToString()) || c.Type.Equals(TransactionType.Deposit.ToString())));
            }

            if (searchField.Equals(TransactionType.ShowHistoryStaff.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.Type.Equals(TransactionType.Withdraw.ToString()));
            }

            if (!string.IsNullOrEmpty(fullname))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.User.FullName.Equals(fullname));
            }

            if (!string.IsNullOrEmpty(phonenumber))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.User.PhoneNumber.Equals(phonenumber));
            }

            if (!string.IsNullOrEmpty(mail))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.User.Email.Equals(mail));
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
                CreatedDate = c.CreatedDate,
                Status = c.Status
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
            if (transact.Status != TransactionStatus.Pending.ToString())
            {
                throw new Exception("Transaction has been processed!");
            }
            if (action)
            {
                //Approve
                transact.Status = TransactionStatus.Done.ToString();
                transact.AfterAmount = (Decimal)user.BalanceWallet - transact.Amount;
                user.BalanceWallet -= (double)transact.Amount;
                transact.Current = (Decimal)user.BalanceWallet;
                _unitOfWork.Repository<WalletTransaction>().Update(transact);
                await _userManager.UpdateAsync(user);
                await _unitOfWork.Repository<WalletTransaction>().SaveChangesAsync();
                return new WalletTransactionWithdrawResponseDTO
                {
                    Amount = (double)transact.Amount,
                    Current = user.BalanceWallet,
                    AfterAmount = (double)transact.AfterAmount,
                    Type = transact.Type,
                    UserId = user.Id,
                    FullName = user.FullName,
                    Mail = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Status = transact.Status,
                    CreatedDate = transact.CreatedDate,
                };
            }
            else
            {
                //Reject
                transact.Status = TransactionStatus.Fail.ToString();
                transact.AfterAmount = (Decimal)user.BalanceWallet;
                transact.Current = (Decimal)user.BalanceWallet;
                _unitOfWork.Repository<WalletTransaction>().Update(transact);
                await _unitOfWork.Repository<WalletTransaction>().SaveChangesAsync();
                return new WalletTransactionWithdrawResponseDTO
                {
                    Amount = (double)transact.Amount,
                    Current = user.BalanceWallet,
                    AfterAmount = (double)transact.AfterAmount,
                    Type = transact.Type,
                    UserId = user.Id,
                    FullName = user.FullName,
                    Mail = user.Email,
                    Status = transact.Status,
                    CreatedDate = transact.CreatedDate
                };
            }
        }
        public async Task<WalletTransactionDepositResponseDTO> processDepositTransaction(decimal amount, ClaimsPrincipal userClaims)
        {
            //var user = await _customerService.GetCustomerAsync(userClaims);
            //if (user == null) throw new Exception(CustomerConst.NotFoundError);
            var user = await _userManager.FindByIdAsync("ccc7f4f1-593c-49b4-8228-10375da7754c");
            var wTransaction = new WalletTransaction
            {
                AfterAmount = (Decimal)user.BalanceWallet + amount,
                Current = (Decimal)user.BalanceWallet,
                User = user,
                UserId = user.Id,
                Amount = amount,
                Type = TransactionType.Deposit.ToString(),
                Status = TransactionStatus.Done.ToString(),
                CreatedDate = DateTime.Now
            };
            user.BalanceWallet += (double)amount;
            await _userManager.UpdateAsync(user);
            await _unitOfWork.Repository<WalletTransaction>().AddAsync(wTransaction);
            await _unitOfWork.SaveChangesAsync();

            return new WalletTransactionDepositResponseDTO
            {
                Amount = (Double)wTransaction.Amount,
                Current = user.BalanceWallet,
                AfterAmount = (Double)wTransaction.AfterAmount,
                Type = wTransaction.Type,
                UserId = user.Id,
                FullName = user.FullName,
                Mail = user.Email,
                PhoneNumber = user.PhoneNumber,
                Status = TransactionStatus.Done.ToString(),
                CreatedDate = DateTime.Now,
                Description = "Deposit success!"
            };
        }

        public async Task<double> getUserBalance(AppUser user)
        {
            return user.BalanceWallet;
        }
        public async Task<double> VNDMoneyExchangeFromUSD(decimal amount)
        {
            ExchangRate exchangRate = new ExchangRate();
            double exchangeRate = exchangRate.GetUsdToVndExchangeRateAsync().Result;
            var AmountInUsd = Convert.ToDouble(amount, CultureInfo.InvariantCulture);
            return Math.Round(exchangRate.ConvertUsdToVnd(AmountInUsd, exchangeRate));
        }
    }
}

