using HCP.DTOs.DTOs;
using HCP.DTOs.DTOs.BookingDTO;
using HCP.DTOs.DTOs.WalletDTO;
using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.Interfaces;
using HCP.Service.Integrations.Currency;
using HCP.Service.Services.CustomerService;
using HCP.Service.Services.EmailService;
using HCP.Service.Services.ListService;
using HCP.Service.Services.TemporaryService;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Security.Claims;

namespace HCP.Service.Services.WalletService
{
    public class WalletService : IWalletService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly ICustomerService _customerService;
        private readonly ITemporaryStorage _temporaryStorage;
        public readonly IEmailSenderService _emailSenderService;

        public WalletService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, ICustomerService customerService, ITemporaryStorage temporaryStorage, IEmailSenderService emailSenderService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _customerService = customerService;
            _temporaryStorage = temporaryStorage;
            _emailSenderService = emailSenderService;
        }
        public async Task<RefundRequestDTO> CreateRefundRequest(Guid bookingId, string ProofOfPayment, string Reason, AppUser user)
        {
            double expectedRefund = 0;
            double availableWithdraw = 0;
            var booking = _unitOfWork.Repository<Booking>().GetById(bookingId);
            if (booking == null) throw new Exception(BookingConst.BookingNotFound);

            var payment = await _unitOfWork.Repository<Payment>().FindAsync(c => c.BookingId == bookingId && c.Status.Equals(TransactionStatus.Done.ToString()));
            var cleaningService = _unitOfWork.Repository<CleaningService>().GetById(booking.CleaningServiceId);
            var housekeeper = await _userManager.FindByIdAsync(cleaningService.UserId);
            if (housekeeper == null) throw new Exception(CustomerConst.NotFoundError);

            expectedRefund += (double)booking.TotalPrice;
            if (housekeeper.BalanceWallet < expectedRefund)
            {
                throw new Exception("Your refund request is more than the housekeeper's balance!");
            }

            var refund = new RefundRequest
            {
                Id = Guid.NewGuid(),
                Reason = Reason,
                AcceptBy = null,
                ProofOfPayment = ProofOfPayment,
                Booking = booking,
                BookingId = booking.Id,
                Staff = null,
                ResolutionDate = null,
                Status = RefundRequestStatus.Pending.ToString(),
            };
            booking.Status = BookingStatus.OnRefunding.ToString();
            await _unitOfWork.Repository<RefundRequest>().AddAsync(refund);
            _unitOfWork.Repository<Booking>().Update(booking);
            await _unitOfWork.SaveChangesAsync();

            return new RefundRequestDTO
            {
                Id = refund.Id,
                Status = refund.Status,
                BookingId = refund.BookingId,
                Reason = refund.Reason,
                ProofOfPayment = refund.ProofOfPayment,
                ResolutionDate = refund.ResolutionDate,
                FullName = user.FullName,
                Mail = user.Email,
                PhoneNumber = user.PhoneNumber,
                AcceptBy = refund.AcceptBy,
            };
        }

        public async Task<RefundRequestShowListDTO> GetRefundRequestsAsync(string? search, int? pageIndex, int? pageSize, string? status)
        {
            var refundRequests = await _unitOfWork.Repository<RefundRequest>()
                .GetAll()
                .Include(rr => rr.Booking)
                    .ThenInclude(b => b.Customer) // Include Customer for name, phone, email
                .Select(rr => new RefundRequestShowDTO
                {
                    Id = rr.Id,
                    BookingId = rr.BookingId,
                    CustomerId = rr.Booking.CustomerId,
                    CustomerName = rr.Booking.Customer.FullName,
                    PhoneNumber = rr.Booking.Customer.PhoneNumber,
                    Email = rr.Booking.Customer.Email,
                    Reason = rr.Reason,
                    Status = rr.Status,
                    AcceptBy = rr.AcceptBy,
                    ResolutionDate = rr.ResolutionDate,
                    TotalPrice = rr.Booking.TotalPrice
                })
                .ToListAsync();
            if (!search.IsNullOrEmpty())
            {
                refundRequests = refundRequests.Where(c => (c.CustomerName.ToLower().Contains(search.ToLower())) || (c.Email.ToLower().Contains(search.ToLower())) || (c.PhoneNumber.ToLower().Contains(search.ToLower()))).ToList();
            }
            if (!status.IsNullOrEmpty())
            {
                refundRequests = refundRequests.Where(c => c.Status.ToLower().Equals(status.ToLower())).ToList();
            }
            if (pageIndex == null || pageSize == null)
            {
                var temp1 = PaginatedList<RefundRequestShowDTO>.CreateAsync(refundRequests, 1, refundRequests.Count());
                return new RefundRequestShowListDTO
                {
                    Items = temp1,
                    hasNext = temp1.HasNextPage,
                    hasPrevious = temp1.HasPreviousPage,
                    totalCount = temp1.TotalCount,
                    totalPages = temp1.TotalPages,
                };
            }
            var temp2 = PaginatedList<RefundRequestShowDTO>.CreateAsync(refundRequests, (int)pageIndex, (int)pageSize);
            return new RefundRequestShowListDTO
            {
                Items = temp2,
                hasNext = temp2.HasNextPage,
                hasPrevious = temp2.HasPreviousPage,
                totalCount = refundRequests.Count(),
                totalPages = temp2.TotalPages,
            };
        }
        public async Task<RefundRequestShowDetailDTO> GetRefundRequestByIdAsync(Guid refundRequestId)
        {
            var refundRequest = await _unitOfWork.Repository<RefundRequest>()
                .GetAll()
                .Include(rr => rr.Booking)
                    .ThenInclude(b => b.Customer)
                .Where(rr => rr.Id == refundRequestId)
                .Select(rr => new RefundRequestShowDetailDTO
                {
                    Id = rr.Id,
                    BookingId = rr.BookingId,
                    CustomerId = rr.Booking.CustomerId,
                    CustomerName = rr.Booking.Customer.FullName,
                    PhoneNumber = rr.Booking.Customer.PhoneNumber,
                    Email = rr.Booking.Customer.Email,
                    Reason = rr.Reason,
                    Status = rr.Status,
                    ProofOfPayment = rr.ProofOfPayment,
                    AcceptBy = rr.AcceptBy,
                    ResolutionDate = rr.ResolutionDate,
                    TotalPrice = rr.Booking.TotalPrice
                })
                .FirstOrDefaultAsync();

            if (refundRequest == null)
            {
                throw new Exception(TransactionConst.NotFoundError);
            }

            return refundRequest;
        }
        public async Task<RefundRequestDTO> StaffProccessRefund(Guid refundId, bool action, AppUser staff)
        {
            var refund = _unitOfWork.Repository<RefundRequest>().GetById(refundId);
            if (refund == null) throw new Exception(TransactionConst.RefundProcessFail);
            var booking = _unitOfWork.Repository<Booking>().GetById(refund.BookingId);
            if (booking == null) throw new Exception(BookingConst.BookingNotFound);
            var user = await _userManager.FindByIdAsync(booking.CustomerId);
            if (user == null) throw new Exception(CustomerConst.NotFoundError);
            var cleaningService = _unitOfWork.Repository<CleaningService>().GetById(booking.CleaningServiceId);
            var housekeeper = await _userManager.FindByIdAsync(cleaningService.UserId) ?? throw new Exception(CommonConst.NotFoundError);


            if (refund.Status != RefundRequestStatus.Pending.ToString())
            {
                throw new Exception(TransactionConst.RefundProcessSuccessfully);
            }
            if (action)
            {
                //Approve
                refund.Status = RefundRequestStatus.Approved.ToString();
                refund.ResolutionDate = DateTime.Now;
                refund.AcceptBy = staff.Id;
                refund.Staff = staff;
                booking.Status = BookingStatus.Refunded.ToString();
                user.BalanceWallet += (double)booking.TotalPrice;
                housekeeper.BalanceWallet -= (double)booking.TotalPrice;
                //Place for system wallet deduce
                _unitOfWork.Repository<RefundRequest>().Update(refund);
                _unitOfWork.Repository<Booking>().Update(booking);
                await _userManager.UpdateAsync(housekeeper);
                await _userManager.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var approveRefundCustomerEmailBody = EmailBodyTemplate.ApproveRefundRequestForCustomer(housekeeper.FullName, user.FullName, refund.BookingId, cleaningService.ServiceName, booking.TotalPrice);
                _emailSenderService.SendEmail(user.Email, RequestConst.RefundEmailCustomerSubject, approveRefundCustomerEmailBody);
                
                var approveRefundHousekeeperEmailBody = EmailBodyTemplate.RejectRefundRequestForHousekeeper(housekeeper.FullName, user.FullName, refund.BookingId, cleaningService.ServiceName, booking.TotalPrice, refund.Reason);
                _emailSenderService.SendEmail(housekeeper.Email, RequestConst.RefundEmailHousekeeperSubject, approveRefundHousekeeperEmailBody);

                return new RefundRequestDTO
                {
                    Id = refund.Id,
                    Status = refund.Status,
                    BookingId = refund.BookingId,
                    Reason = refund.Reason,
                    ProofOfPayment = refund.ProofOfPayment,
                    ResolutionDate = refund.ResolutionDate,
                    FullName = user.FullName,
                    Mail = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    AcceptBy = refund.AcceptBy,
                };
            }
            else
            {
                //Reject
                refund.Status = RefundRequestStatus.Rejected.ToString();
                refund.ResolutionDate = DateTime.Now;
                refund.AcceptBy = staff.Id;
                refund.Staff = staff;
                booking.Status = BookingStatus.RefundRejected.ToString();
                _unitOfWork.Repository<RefundRequest>().Update(refund);
                _unitOfWork.Repository<Booking>().Update(booking);
                await _unitOfWork.Repository<RefundRequest>().SaveChangesAsync();

                var rejectRefundCustomerEmailBody = EmailBodyTemplate.RejectRefundRequestForCustomer(housekeeper.FullName, user.FullName, refund.BookingId, cleaningService.ServiceName, booking.TotalPrice, refund.Reason);
                _emailSenderService.SendEmail(user.Email, RequestConst.RefundEmailCustomerSubject, rejectRefundCustomerEmailBody);

                return new RefundRequestDTO
                {
                    Id = refund.Id,
                    Status = refund.Status,
                    BookingId = refund.BookingId,
                    Reason = refund.Reason,
                    ProofOfPayment = refund.ProofOfPayment,
                    ResolutionDate = refund.ResolutionDate,
                    FullName = user.FullName,
                    Mail = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    AcceptBy = refund.AcceptBy,
                };
            }
        }

        public async Task<WalletWithdrawRequestDTO> CreateWithdrawRequest(decimal amount, AppUser user)
        {
            double expectedWithdraw = 0;
            double availableWithdraw = 0;
            var pendingTransact = _unitOfWork.Repository<WalletTransaction>().GetAll().Where(c => (c.User == user && c.Status.Equals(TransactionStatus.Pending.ToString())));
            foreach (var transaction in pendingTransact)
            {
                expectedWithdraw += (double)transaction.Amount;
            }
            if (user.BalanceWallet < (expectedWithdraw + (double)amount))
            {
                availableWithdraw = user.BalanceWallet - (expectedWithdraw);
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
                Type = TransactionType.Withdraw.ToString(),
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
            var wTransactionList = _unitOfWork.Repository<WalletTransaction>().GetAll()
                .OrderByDescending(c => c.CreatedDate);
            if (searchField.Equals(TransactionType.WithdrawRequestUser.ToString())
                || searchField.Equals(TransactionType.WithdrawUser.ToString())
                || searchField.Equals(TransactionType.WithdrawRejectUser.ToString())
                || searchField.Equals(TransactionType.ShowAllHistoryUser.ToString())
                || searchField.Equals(TransactionType.ShowWithdrawHistoryUser.ToString())
                || searchField.Equals(TransactionType.Deposit.ToString())
                || searchField.Equals(TransactionType.BookingPurchase.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.User.Id == user.Id);
            }

            if (searchField.Equals(TransactionType.WithdrawRequestStaff.ToString())
                || searchField.Equals(TransactionType.WithdrawRequestUser.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.Type.Equals(TransactionType.Withdraw.ToString())
                    && c.Status.Equals(TransactionStatus.Pending.ToString()));
            }

            if (searchField.Equals(TransactionType.Deposit.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.Type.Equals(TransactionType.Deposit.ToString())
                    && c.Status.Equals(TransactionStatus.Done.ToString()));
            }

            if (searchField.Equals(TransactionType.WithdrawStaff.ToString())
                || searchField.Equals(TransactionType.WithdrawUser.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.Type.Equals(TransactionType.Withdraw.ToString())
                    && c.Status.Equals(TransactionStatus.Done.ToString()));
            }

            if (searchField.Equals(TransactionType.WithdrawRejectStaff.ToString())
                || searchField.Equals(TransactionType.WithdrawRejectUser.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.Type.Equals(TransactionType.Withdraw.ToString())
                    && c.Status.Equals(TransactionStatus.Fail.ToString()));
            }

            if (searchField.Equals(TransactionType.ShowWithdrawHistoryUser.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.Type.Equals(TransactionType.Withdraw.ToString()));
            }

            if (searchField.Equals(TransactionType.BookingPurchase.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => c.Type.Equals(TransactionType.BookingPurchase.ToString()));
            }

            if (searchField.Equals(TransactionType.ShowAllHistoryUser.ToString()))
            {
                wTransactionList = (IOrderedQueryable<WalletTransaction>)wTransactionList
                    .Where(c => (c.Type.Equals(TransactionType.Withdraw.ToString()) || c.Type.Equals(TransactionType.Deposit.ToString()) || c.Type.Equals(TransactionType.BookingPurchase.ToString())));
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
            if (user == null) throw new Exception(CustomerConst.NotFoundError);
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

        //This method is used to create a deposit transaction, store it in the temporary storage and return the transaction
        public async Task<WalletTransactionDepositResponseDTO> createDepositTransaction(decimal amount, AppUser user)
        {
            Guid id = Guid.NewGuid();
            var wTransaction = new WalletTransaction
            {
                Id = id,
                AfterAmount = 0,
                Current = (Decimal)user.BalanceWallet,
                User = user,
                UserId = user.Id,
                Amount = amount,
                Type = TransactionType.Deposit.ToString(),
                Status = TransactionStatus.Pending.ToString(),
                CreatedDate = DateTime.Now
            };
            await _unitOfWork.Repository<WalletTransaction>().AddAsync(wTransaction);
            await _unitOfWork.SaveChangesAsync();

            return new WalletTransactionDepositResponseDTO
            {
                Id = id,
                Amount = (Double)wTransaction.Amount,
                Current = (Double)wTransaction.Current,
                AfterAmount = (Double)wTransaction.AfterAmount,
                Type = wTransaction.Type,
                UserId = user.Id,
                FullName = user.FullName,
                Mail = user.Email,
                PhoneNumber = user.PhoneNumber,
                Status = wTransaction.Status,
                CreatedDate = wTransaction.CreatedDate,
                Description = "Deposit pending!"
            };
        }

        //This method is used to process the deposit transaction, take the transaction from the temporary storage and process it, including updating the user's balance
        public async Task<WalletTransactionDepositResponseDTO> processDepositTransaction(Guid depoTrans, bool successOrNot)
        {
            if (successOrNot)
            {
                var wTransaction = await _unitOfWork.Repository<WalletTransaction>().FindAsync(c => c.Id.Equals(depoTrans));
                if (wTransaction == null) throw new Exception(TransactionConst.NotFoundError);
                var user = await _userManager.FindByIdAsync(wTransaction.UserId);
                if (user == null) throw new Exception(CustomerConst.NotFoundError);
                wTransaction.AfterAmount = (Decimal)user.BalanceWallet + wTransaction.Amount;
                wTransaction.Current = (Decimal)user.BalanceWallet;
                wTransaction.Status = TransactionStatus.Done.ToString();
                user.BalanceWallet += (double)wTransaction.Amount;
                await _userManager.UpdateAsync(user);
                _unitOfWork.Repository<WalletTransaction>().Update(wTransaction);
                await _unitOfWork.SaveChangesAsync();

                return new WalletTransactionDepositResponseDTO
                {
                    Id = wTransaction.Id,
                    Amount = (Double)wTransaction.Amount,
                    Current = (Double)wTransaction.Current,
                    AfterAmount = (Double)wTransaction.AfterAmount,
                    Type = wTransaction.Type,
                    UserId = user.Id,
                    FullName = user.FullName,
                    Mail = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Status = wTransaction.Status,
                    CreatedDate = wTransaction.CreatedDate,
                    Description = "Deposit success!"
                };
            }
            else
            {
                var wTransaction = await _unitOfWork.Repository<WalletTransaction>().FindAsync(c => c.Id.Equals(depoTrans));
                if (wTransaction == null) throw new Exception(TransactionConst.NotFoundError);
                var user = wTransaction.User;
                wTransaction.AfterAmount = (Decimal)user.BalanceWallet;
                wTransaction.Current = (Decimal)user.BalanceWallet;
                wTransaction.Status = TransactionStatus.Fail.ToString();
                await _unitOfWork.SaveChangesAsync();
                return new WalletTransactionDepositResponseDTO
                {
                    Id = wTransaction.Id,
                    Amount = (Double)wTransaction.Amount,
                    Current = (Double)wTransaction.Current,
                    AfterAmount = (Double)wTransaction.AfterAmount,
                    Type = wTransaction.Type,
                    UserId = user.Id,
                    FullName = user.FullName,
                    Mail = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Status = wTransaction.Status,
                    CreatedDate = wTransaction.CreatedDate,
                    Description = "Deposit failed!"
                };
            }
        }

        public async Task<double> getUserBalance(AppUser user)
        {
            return user.BalanceWallet;
        }
        public async Task DeduceFromWallet(ClaimsPrincipal user, decimal amount)
        {
            var user1 = await _userManager.GetUserAsync(user);
            var wallet = _unitOfWork.Repository<SystemWallet>().GetAll().FirstOrDefault();
            if (wallet == null)
            {
                wallet = new SystemWallet
                {
                    Balance = 0.0m,
                };
                await _unitOfWork.Repository<SystemWallet>().AddAsync(wallet);
            }

            WalletTransaction wTransaction = new WalletTransaction
            {
                Id = Guid.NewGuid(),
                AfterAmount = 0,
                Current = (Decimal)user1.BalanceWallet,
                User = user1,
                UserId = user1.Id,
                Amount = amount,
                Type = TransactionType.BookingPurchase.ToString(),
                Status = TransactionStatus.Done.ToString(),
                CreatedDate = DateTime.Now
            };

            user1.BalanceWallet -= (double)amount;
            wTransaction.AfterAmount = (Decimal)user1.BalanceWallet;
            wallet.Balance += amount;
            await _userManager.UpdateAsync(user1);
            await _unitOfWork.Repository<WalletTransaction>().AddAsync(wTransaction);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeduceFromWallet1(AppUser user1, decimal amount)
        {
            var wallet = _unitOfWork.Repository<SystemWallet>().GetAll().FirstOrDefault();
            if (wallet == null)
            {
                wallet = new SystemWallet
                {
                    Balance = 0.0m,
                };
                await _unitOfWork.Repository<SystemWallet>().AddAsync(wallet);
            }

            WalletTransaction wTransaction = new WalletTransaction
            {
                Id = Guid.NewGuid(),
                AfterAmount = 0,
                Current = (Decimal)user1.BalanceWallet,
                User = user1,
                UserId = user1.Id,
                Amount = amount,
                Type = TransactionType.BookingPurchase.ToString(),
                Status = TransactionStatus.Done.ToString(),
                CreatedDate = DateTime.Now
            };

            user1.BalanceWallet -= (double)amount;
            wTransaction.AfterAmount = (Decimal)user1.BalanceWallet;
            wallet.Balance += amount;
            await _userManager.UpdateAsync(user1);
            await _unitOfWork.Repository<WalletTransaction>().AddAsync(wTransaction);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<double> VNDMoneyExchangeFromUSD(decimal amount)
        {
            ExchangRate exchangRate = new ExchangRate();
            double exchangeRate = exchangRate.GetUsdToVndExchangeRateAsync().Result;
            var AmountInUsd = Convert.ToDouble(amount, CultureInfo.InvariantCulture);
            return Math.Round(exchangRate.ConvertUsdToVnd(AmountInUsd, exchangeRate));
        }

        public async Task<double> getUserBalance(ClaimsPrincipal user)
        {
            var user1 = await _userManager.GetUserAsync(user);
            return user1.BalanceWallet;
        }

        public async Task<RevenueHousekeeperDatasListShowDTO> GetRevenueHousekeeperDatas(AppUser user, bool dayRevenue, bool weekRevenue, bool yearRevenue, bool yearsRevenue, int? dayStart, int? monthStart, int? yearStart, int? dayEnd, int? monthEnd, int? yearEnd)
        {
            var chartDataList = new List<RevenueHousekeeperDatasShowDTO>();

            // Query payments with status "Completed" and include booking
            var payments = await _unitOfWork.Repository<Payment>()
                .GetAll()
                .Where(p => p.Status == "succeed" && (p.Booking.Status.Equals(BookingStatus.Completed.ToString()) || p.Booking.Status.Equals(BookingStatus.RefundRejected.ToString())))
                .Include(p => p.Booking).Include(p => p.Booking.CleaningService)
                .ToListAsync();
            payments = payments.Where(p => p.Booking.CleaningService?.UserId == user.Id).ToList();
            //payments where booking refunded
            var paymentsRefunded = await _unitOfWork.Repository<Payment>()
                .GetAll()
                .Where(p => p.Status == "succeed" && p.Booking.Status.Equals(BookingStatus.Refunded.ToString()))
                .Include(p => p.Booking).Include(p => p.Booking.CleaningService)
                .ToListAsync();
            paymentsRefunded = paymentsRefunded.Where(p => p.Booking.CleaningService?.UserId == user.Id).ToList();
            if (yearRevenue && yearStart.HasValue)
            {
                // Year chart - show data for each month in the selected year
                for (int month = 1; month <= 12; month++)
                {
                    // Define the start and end of the month
                    var monthStartDate = new DateTime(yearStart.Value, month, 1);
                    var monthEndDate = monthStartDate.AddMonths(1).AddDays(-1);
                    var monthlyRevenue = (payments.Where(p => p.Booking.CompletedAt >= monthStartDate && p.Booking.CompletedAt <= monthEndDate).Sum(p => p.Amount) * 0.9m)/* - 
                        (paymentsRefunded.Where(p => p.Booking.CompletedAt >= monthStartDate && p.Booking.CompletedAt <= monthEndDate).Sum(p => p.Amount))*/; // 90% of the amount
                    chartDataList.Add(new RevenueHousekeeperDatasShowDTO
                    {
                        name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                        revenue = (double)monthlyRevenue // Assuming 'revenued' is the correct property name
                    });
                }
            }

            else if (yearsRevenue && yearStart.HasValue && yearEnd.HasValue)
            {
                // Years chart - show data for each year between start and end year
                for (int year = yearStart.Value; year <= yearEnd.Value; year++)
                {
                    var yearStartDate = new DateTime(year, 1, 1);
                    var yearEndDate = new DateTime(year, 12, 31);

                    var yearlyRevenue = (payments
                        .Where(p => p.Booking.CompletedAt >= yearStartDate && p.Booking.CompletedAt <= yearEndDate)
                        .Sum(p => p.Amount) * 0.9m)/* -
                        (paymentsRefunded
                        .Where(p => p.Booking.CompletedAt >= yearStartDate && p.Booking.CompletedAt <= yearEndDate)
                        .Sum(p => p.Amount))*/; // 90% of the amount

                    chartDataList.Add(new RevenueHousekeeperDatasShowDTO
                    {
                        name = year.ToString(),
                        revenue = (double)yearlyRevenue
                    });
                }
            }
            else if (dayRevenue && !weekRevenue && !yearRevenue && !yearsRevenue &&
                     dayStart.HasValue && monthStart.HasValue && yearStart.HasValue &&
                     dayEnd.HasValue && monthEnd.HasValue && yearEnd.HasValue)
            {
                // Day chart - show data for each day between start and end date
                var startDate = new DateTime(yearStart.Value, monthStart.Value, dayStart.Value);
                var endDate = new DateTime(yearEnd.Value, monthEnd.Value, dayEnd.Value);
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var dailyRevenue = (payments
                        .Where(p => p.Booking.CompletedAt.HasValue && p.Booking.CompletedAt.Value.Date == date.Date)
                        .Sum(p => p.Amount) * 0.9m)/* -
                        (paymentsRefunded
                        .Where(p => p.Booking.CompletedAt.HasValue && p.Booking.CompletedAt.Value.Date == date.Date)
                        .Sum(p => p.Amount))*/; // 10% of the amount

                    chartDataList.Add(new RevenueHousekeeperDatasShowDTO
                    {
                        name = date.ToString("dd/MM/yyyy"),
                        revenue = (double)dailyRevenue
                    });
                }
            }
            return new RevenueHousekeeperDatasListShowDTO
            {
                ChartData = chartDataList
            };
        }
    }
}

