﻿using HCP.DTOs.DTOs.BookingDTO;
using HCP.DTOs.DTOs.WalletDTO;
using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Service.Services.CustomerService;
using HCP.Service.Services.WalletService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICustomerService _customerService;

        public WalletController(IWalletService walletService, UserManager<AppUser> userManager, ICustomerService customerService)
        {
            _walletService = walletService;
            _userManager = userManager;
            _customerService = customerService;
        }
        [HttpGet()]
        [Authorize]
        public async Task<IActionResult> getTransactPaging(string transactionType, int? pageIndex, int? pageSize, string? fullname, string? phonenumber, string? mail)
        {
            var user = await _customerService.GetCustomerAsync(User);
            if (user != null)
            {
                var list = await _walletService.GetTransacts(user, pageIndex, pageSize, transactionType, fullname, phonenumber, mail);
                return Ok(new AppResponse<GetWalletWithdrawRequestListDTO>().SetSuccessResponse(list));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse(KeyConst.NotFound, CommonConst.NotFoundError));
        }
        [HttpPost("sendWithdrawRequest")]
        [Authorize]
        public async Task<IActionResult> createWithdrawRequest(decimal amount)
        {
            var user = await _customerService.GetCustomerAsync(User);
            if (user != null)
            {
                var withdrawRequest = await _walletService.CreateWithdrawRequest(amount, user);
                return Ok(new AppResponse<WalletWithdrawRequestDTO>().SetSuccessResponse(withdrawRequest));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse(KeyConst.Error, CommonConst.SomethingWrongMessage));
        }
        [HttpPost("sendRefundRequest")]
        [Authorize(Roles = KeyConst.Customer)]
        public async Task<IActionResult> createRefundRequest(Guid bookingId, string ProofOfPayment, string Reason)
        {
            var user = await _customerService.GetCustomerAsync(User);
            if (user != null)
            {
                var refundRequest = await _walletService.CreateRefundRequest(bookingId, ProofOfPayment, Reason, user);
                return Ok(new AppResponse<RefundRequestDTO>().SetSuccessResponse(refundRequest));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse(KeyConst.Error, TransactionConst.RefundFail));
        }
        [HttpGet("getRefundRequest")]
        [Authorize(Roles = KeyConst.Staff)]
        public async Task<IActionResult> getRefundPaging(string? search, int? pageIndex, int? pageSize, string? status)
        {
            var list = await _walletService.GetRefundRequestsAsync(search, pageIndex, pageSize, status);
            return Ok(new AppResponse<RefundRequestShowListDTO>().SetSuccessResponse(list));
        }
        [HttpGet("getRefundRequestDetail")]
        [Authorize(Roles = KeyConst.Staff)]
        public async Task<IActionResult> getRefundDetail(Guid refundRequestId)
        {
            var list = await _walletService.GetRefundRequestByIdAsync(refundRequestId);
            return Ok(new AppResponse<RefundRequestShowDetailDTO>().SetSuccessResponse(list));
        }
        [HttpPost("processRefund")]
        [Authorize(Roles = KeyConst.Staff)]
        public async Task<IActionResult> processRefund(Guid refundRequestId, bool action)
        {
            var user = await _customerService.GetCustomerAsync(User);
            if (user != null)
            {
                var refund = await _walletService.StaffProccessRefund(refundRequestId, action, user);
                return Ok(new AppResponse<RefundRequestDTO>().SetSuccessResponse(refund));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse(KeyConst.Error, TransactionConst.RefundFail));
        }
        [HttpPost("processWithdraw")]
        [Authorize(Roles = KeyConst.Staff)]
        public async Task<IActionResult> processWithdraw(Guid transId, bool action)
        {
            var withdraw = await _walletService.StaffProccessWithdraw(transId, action);
            return Ok(new AppResponse<WalletTransactionWithdrawResponseDTO>().SetSuccessResponse(withdraw));
        }
        [HttpGet("balance")]
        [Authorize]
        public async Task<IActionResult> balanceShow()
        {
            var user = await _customerService.GetCustomerAsync(User);
            if (user != null)
            {
                var deposit = await _walletService.getUserBalance(user);
                return Ok(new AppResponse<double>().SetSuccessResponse(deposit));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse(KeyConst.Error, CommonConst.SomethingWrongMessage));
        }
        [HttpGet("moneyExchange")]
        [Authorize]
        public async Task<IActionResult> moneyExchange(decimal amount)
        {
            var returnAmount = await _walletService.VNDMoneyExchangeFromUSD(amount);
            return Ok(new AppResponse<double>().SetSuccessResponse(returnAmount));
        }
        [HttpGet("revenueHousekeeperData")]
        [Authorize(Roles = KeyConst.Housekeeper)]
        public async Task<IActionResult> GetRevenueHousekeeperData(bool dayRevenue, bool weekRevenue, bool yearRevenue, bool yearsRevenue, int? dayStart, int? monthStart, int? yearStart, int? dayEnd, int? monthEnd, int? yearEnd)
        {
            var user = await _customerService.GetCustomerAsync(User);
            if (user != null)
            {
                var chartData = await _walletService.GetRevenueHousekeeperDatas(user, dayRevenue, weekRevenue, yearRevenue, yearsRevenue, dayStart, monthStart, yearStart, dayEnd, monthEnd, yearEnd);
                return Ok(new AppResponse<RevenueHousekeeperDatasListShowDTO>().SetSuccessResponse(chartData));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse(KeyConst.Error, CommonConst.NotFoundError));
        }
    }
}
