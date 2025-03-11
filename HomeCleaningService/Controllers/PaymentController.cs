using HCP.Repository.Constance;
using HCP.Repository.Entities;
using HCP.Service.DTOs.BookingDTO;
using HCP.Service.Integrations.Vnpay;
using HCP.Service.Services.BookingService;
using HCP.Service.Services.EmailService;
using HCP.Service.Services.WalletService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

        public PaymentController(UserManager<AppUser> userManager, IBookingService bookingService, Ivnpay ivnpay, IEmailSenderService emailSenderService, IConfiguration configuration, IWalletService walletService)
        {
            _vnpHashSecret = configuration["VNPay:HashSecret"];
            _userManager = userManager;
            _bookingService = bookingService;
            this.ivnpay = ivnpay;
            _emailSenderService = emailSenderService;
            _walletService = walletService;
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
        public async Task<IActionResult> CreateDepositPayment(int amount, string paymentMethod = KeyConst.VnPay)
        {
            var userClaims = User;
            try
            {
                // Generate the VNPay payment URL
                var returnUrl = "https://your-return-url.com";
                string paymentUrl = ivnpay.CreateDepositPaymentUrl(amount, returnUrl);

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
        public async Task<IActionResult> CreatePayment([FromBody] ConfirmBookingDTO request, string paymentMethod = KeyConst.VnPay)
        {
            var userClaims = User;
            try
            {
                // Create the booking
                var booking = await _bookingService.CreateBookingAsync(request, userClaims);
                await _bookingService.CreatePayment(booking.Id, booking.TotalPrice, paymentMethod);
                // Generate the VNPay payment URL
                var returnUrl = "https://your-return-url.com";
                string paymentUrl = ivnpay.CreatePaymentUrl(booking, returnUrl);

                return Ok(new { bookingId = booking.Id, url = paymentUrl });
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

        [Authorize]
        [HttpGet("PaymentReturn-VNPAY")]
        public IActionResult PaymentReturn()
        {
            string queryString = Request.QueryString.Value;
            //var vnp_HashSecret = "DIGHI9T61AVLTF4C28ZTV6BX4HKI027T";
            var vnp_HashSecret = _vnpHashSecret;
            // Retrieve the order ID from the query string
            if (Guid.TryParse(Request.Query["vnp_TxnRef"], out Guid orderId))
            {
                if (true)
                {
                    var paymentStatus = Request.Query["vnp_ResponseCode"];
                    if (paymentStatus == PaymentConst.SuccessCode)                                  //"00" means success
                    {

                        //return Redirect("https://www.google.com/");                                   // Redirect to success page
                        return Redirect("http://localhost:5173/service/Checkout/success");
                    }
                    else
                    {
                        _bookingService.UpdateStatusBooking(orderId, PaymentConst.IsDeleted);
                        return Redirect("http://localhost:5173/service/Checkout/fail");
                    }
                }
            }
            return BadRequest(PaymentConst.InvalidError);
        }
        [HttpGet("PaymentDepositReturn-VNPAY")]
        public async Task<IActionResult> PaymentDepositReturn()
        {
            var userClaims = User;
            string queryString = Request.QueryString.Value;

            var paymentStatus = Request.Query["vnp_ResponseCode"];
            if (paymentStatus == "00") //"00" means success
            {
                string orderInfo = Request.Query["vnp_OrderInfo"];

                decimal usdAmount;

                if (decimal.TryParse(orderInfo, out usdAmount))
                {
                    await _walletService.processDepositTransaction(usdAmount, userClaims);
                    return Redirect("http://localhost:5173/wallet/deposit/success");
                }

                return BadRequest("Invalid amount format in order info.");
            }
            else
            {
                return Redirect("http://localhost:5173/wallet/deposit/fail");
            }
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
