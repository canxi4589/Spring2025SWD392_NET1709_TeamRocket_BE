using HCP.DTOs.DTOs.PaymentDTO;
using HCP.Repository.Entities;
using static HCP.Service.Integrations.Vnpay.VnPay;

namespace HCP.Service.Integrations.Vnpay
{
    public interface Ivnpay
    {
        string CreateDepositPaymentUrl(WalletTransaction walletTrans);

        string CreatePaymentUrl(Booking order);
        string CreatePaymentUrl(PaymentBodyDTO body);
        bool ValidateSignature(string queryString, string vnp_HashSecret);
        Task<HttpResponseMessage> SendRefundRequestAsync(VnpayRefundRequest request, string url);
        string GenerateSecureHash(VnpayRefundRequest request, string secretKey);
        string CreatePaymentUrl1(decimal amount, string returnUrl);
    }
}
