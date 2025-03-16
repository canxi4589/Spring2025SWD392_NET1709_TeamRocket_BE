using HCP.Repository.Entities;
using HCP.Service.DTOs.CheckoutDTO;
using HCP.Service.DTOs.PaymentDTO;
using HCP.Service.DTOs.WalletDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HCP.Service.Integrations.Vnpay.VnPay;

namespace HCP.Service.Integrations.Vnpay
{
    public interface Ivnpay
    {
        string CreateDepositPaymentUrl(WalletTransaction walletTrans, string returnUrl);

        string CreatePaymentUrl(Booking order);
        string CreatePaymentUrl(PaymentBodyDTO body);
        bool ValidateSignature(string queryString, string vnp_HashSecret);
        Task<HttpResponseMessage> SendRefundRequestAsync(VnpayRefundRequest request, string url);
        string GenerateSecureHash(VnpayRefundRequest request, string secretKey);
        string CreatePaymentUrl1(decimal amount, string returnUrl);
    }
}
