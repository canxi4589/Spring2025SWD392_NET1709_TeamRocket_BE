using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs.WalletDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HCP.Service.Services.WalletService
{
    public class WalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> userManager;

        public WalletService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            this.userManager = userManager;
        }

        ////Ham xu ly tien ra tien vao, 1 la nap 2 la rut
        //public async Task<IActionResult> processWalletTransaction(int type, AppUser user)

        public async Task<WalletWithdrawRequestDTO> CreateWithdrawRequest(decimal amount, AppUser user)
        {
            //if (user.BalanceWallet < amount) throw new Exception("Your balance is lower than the amount you wish to withdraw!");
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
                User = user,
                UserId = user.Id
            };
        }

    }
}
