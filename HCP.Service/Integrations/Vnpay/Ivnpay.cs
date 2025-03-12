using HCP.Repository.Entities;
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
        string CreatePaymentUrl(Booking order, string returnUrl);
        string CreateDepositPaymentUrl(WalletTransaction walletTrans, string returnUrl);

        string CreatePaymentUrl(Booking order);
        string CreateDepositPaymentUrl(int amount, string returnUrl);
        bool ValidateSignature(string queryString, string vnp_HashSecret);
        Task<HttpResponseMessage> SendRefundRequestAsync(VnpayRefundRequest request, string url);
        string GenerateSecureHash(VnpayRefundRequest request, string secretKey);
        string CreatePaymentUrl1(decimal amount, string returnUrl);
    }
}
