using HCP.Repository.Entities;
using static Services.Charge.VnPay;

namespace Services.Charge
{
    public interface Ivnpay
    {
        string CreatePaymentUrl(Booking booking, string returnUrl);
        public string CreatePayment(Booking booking, string returnUrl);
        bool ValidateSignature(string queryString, string vnp_HashSecret);
        Task<HttpResponseMessage> SendRefundRequestAsync(VnpayRefundRequest request, string url);
        string GenerateSecureHash(VnpayRefundRequest request, string secretKey);
    }
}
