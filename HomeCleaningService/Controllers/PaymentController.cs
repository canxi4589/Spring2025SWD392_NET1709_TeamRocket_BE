using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Repository.Interfaces;
using HCP.Service.DTOs;
using HCP.Service.DTOs.BookingDTO;
using HCP.Service.DTOs.CheckoutDTO;
using HCP.Service.DTOs.PaymentDTO;
using HCP.Service.Integrations.Vnpay;
using HCP.Service.Services.BookingService;
using HCP.Service.Services.CheckoutService;
using HCP.Service.Services.CustomerService;
using HCP.Service.Services.EmailService;
using HCP.Service.Services.TemporaryService;
using HCP.Service.Services.WalletService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly UserManager<AppUser> _userManager;
        private readonly Ivnpay ivnpay;
        private readonly IEmailSenderService _emailSenderService;
        private readonly string _vnpHashSecret;
        private readonly IWalletService _walletService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICustomerService _customerService;
        private readonly ITemporaryStorage _tempStorage;
        private readonly ICheckoutService _checkoutService;

        public PaymentController(IBookingService bookingService, UserManager<AppUser> userManager, Ivnpay ivnpay, IEmailSenderService emailSenderService, string vnpHashSecret, IWalletService walletService, IUnitOfWork unitOfWork, ICustomerService customerService, ITemporaryStorage tempStorage, ICheckoutService checkoutService)
        {
            _bookingService = bookingService;
            _userManager = userManager;
            this.ivnpay = ivnpay;
            _emailSenderService = emailSenderService;
            _vnpHashSecret = vnpHashSecret;
            _walletService = walletService;
            _unitOfWork = unitOfWork;
            _customerService = customerService;
            _tempStorage = tempStorage;
            _checkoutService = checkoutService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GetCheckout([FromBody] CheckoutRequestDTO request)
        {
            var response = new AppResponse<CheckoutResponseDTO>();

            try
            {
                var claim = User;
                var checkoutInfo = await _bookingService.GetCheckoutInfo(request, claim);
                return Ok(response.SetSuccessResponse(checkoutInfo));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(response.SetErrorResponse(KeyConst.Checkout, ex.Message));
            }
            catch (Exception ex)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.Checkout, ex.Message));
            }
        }
        [HttpPost("CreateDepositPayment")]
        [Authorize]
        public async Task<IActionResult> CreateDepositPaymentAsync(int amount, string paymentMethod = KeyConst.VnPay)
        {
            var user = await _customerService.GetCustomerAsync(User);
            if (user == null)
            {
                return Unauthorized("User not found.");
            }
            try
            {
                // Generate the VNPay payment URL
                var returnUrl = "https://your-return-url.com";
                var depoTransac = await _walletService.createDepositTransaction(amount, user);
                var walletTrans = _unitOfWork.Repository<WalletTransaction>().GetById(depoTransac.Id);
                if (walletTrans == null) throw new Exception(TransactionConst.DepositFail);
                string paymentUrl = ivnpay.CreateDepositPaymentUrl(walletTrans, returnUrl);

                return Ok(new { url = paymentUrl });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new AppResponse<string>().SetErrorResponse(KeyConst.Unathorized, ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new AppResponse<string>().SetErrorResponse(KeyConst.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AppResponse<string>().SetErrorResponse(KeyConst.Error, ex.Message));
            }
        }
        [HttpPost("CreatePayment")]
        [Authorize]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentBodyDTO dto)
        {
            var userClaims = User;
            try
            {

                if (dto.PaymentMethod.ToUpper() == "WALLET")
                {
                    var userId = userClaims.FindFirst("id")?.Value;
                    if (string.IsNullOrEmpty(userId))
                        throw new UnauthorizedAccessException("User ID not found in claims");

                    var walletBalance = await _walletService.getUserBalance(User);
                    if (walletBalance < (double)dto.Amount)
                        return BadRequest(new AppResponse<string>()
                            .SetErrorResponse("INSUFFICIENT_FUNDS", "Not enough funds in wallet"));

                    await _walletService.DeduceFromWallet(User, dto.Amount);
                    //de transaction cho Thinh lam

                    return Ok(new { url = $"http://localhost:5173/service/Checkout/success" });
                }
                else 
                {
                    await _tempStorage.StoreAsync(dto);
                    dto.Uid = userClaims.FindFirst("id")?.Value;
                    string paymentUrl = ivnpay.CreatePaymentUrl(dto);
                    return Ok(new { url = paymentUrl });
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new AppResponse<string>().SetErrorResponse(KeyConst.Unathorized, ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new AppResponse<string>().SetErrorResponse(KeyConst.NotFound, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AppResponse<string>().SetErrorResponse(KeyConst.Error, ex.Message));
            }
        }

        [HttpGet("PaymentReturn-VNPAY")]
        public async Task<IActionResult> PaymentReturn()
        {
            string queryString = Request.QueryString.Value;
            var paymentStatus = Request.Query["vnp_ResponseCode"];
            if (Guid.TryParse(Request.Query["vnp_TxnRef"], out Guid Id))
            {
                if (paymentStatus == PaymentConst.SuccessCode)
                {
                    var dto =await _tempStorage.RetrieveAsync(Id);
                    var body =await _checkoutService.GetCheckoutById(Id);
                    var booking = await _bookingService.CreateBookingAsync1(body,dto.Uid);
                    await _bookingService.CreatePayment(booking.Id,booking.TotalPrice,dto.PaymentMethod);
                    var user = await _userManager.FindByIdAsync(dto.Uid);
                    _emailSenderService.SendEmail(user.Email, "Thank you for using our services", EmailBodyTemplate.GetThankYouEmail(user.FullName));
                    //return Redirect("https://www.google.com/");                                 
                    return Redirect("http://localhost:5173/service/Checkout/success");
                }
            }
            return Redirect("http://localhost:5173/service/Checkout/fail");


        }
        [HttpGet("PaymentDepositReturn-VNPAY")]
        public async Task<IActionResult> PaymentDepositReturn()
        {
            string queryString = Request.QueryString.Value;
            var vnp_HashSecret = _vnpHashSecret;
            if (Guid.TryParse(Request.Query["vnp_TxnRef"], out Guid transactId))
            {
                if (true)
                {
                    var paymentStatus = Request.Query["vnp_ResponseCode"];
                    if (paymentStatus == PaymentConst.SuccessCode)                                  //"00" means success
                    {
                        await _walletService.processDepositTransaction(transactId, true);
                        //return Redirect("https://www.google.com/");                                   // Redirect to success page
                        return Redirect("http://localhost:5173/wallet/deposit/success");
                    }
                    else
                    {
                        await _walletService.processDepositTransaction(transactId, false);
                        return Redirect("http://localhost:5173/wallet/deposit/fail");
                    }
                }
            }
            return BadRequest(PaymentConst.InvalidError);
        }

        //[HttpPost("CreateBooking")]
        //public Task<IActionResult> CreateBooking([FromBody] BookingDTO bookingDto, string paymentType)
        //{
        //    if (bookingDto == null || bookingDto.TotalPrice <= 0)
        //    {
        //        return BadRequest("Invalid booking data.");
        //    }

        //    try
        //    {
        //        var bookingId = await _bookingService.CreateBooking(bookingDto);
        //        var booking = await _unitOfWork.Repository<Booking>().FindAsync(b => b.Id == bookingId);

        //        if (booking == null)
        //        {
        //            return StatusCode(500, "Failed to create booking.");
        //        }
        //        if (paymentType == "Wallet")
        //        {
        //            var wallet = await _walletService.GetWalletByCustomerIdAsync(booking.CustomerId);
        //            if (wallet == null || wallet.Balance < booking.TotalPrice)
        //            {
        //                return BadRequest("Insufficient wallet balance.");
        //            }

        //            // Deduct balance & update wallet
        //            wallet.Balance -= booking.TotalPrice;
        //            await _walletService.UpdateWalletAsync(wallet);

        //            // Mark booking as paid
        //            booking.Status = "Paid";
        //            booking.CompletedAt = DateTime.UtcNow;
        //            await _unitOfWork.Repository<Booking>().UpdateAsync(booking);
        //            await _unitOfWork.SaveChangesAsync();

        //            return Ok(new { message = "Booking successfully paid with wallet.", bookingId });
        //        }
        //        else if (paymentType == "VNPay")
        //        {
        //            var returnUrl = "https://cosmodiamond.xyz/payment-return";
        //            string paymentUrl = _vnPayService.CreatePayment(booking, returnUrl);

        //            return Ok(new { url = paymentUrl, bookingId });
        //        }
        //        else
        //        {
        //            return BadRequest("Invalid payment type.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = ex.Message });
        //    }
        //}

    }
}
