﻿using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Net.Http.Headers;
using HCP.Repository.Entities;
using Newtonsoft.Json;

namespace Services.Charge
{
    public class VnPay : Ivnpay
    {
        private readonly string vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;


        public VnPay(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;   
             vnp_HashSecret = _configuration["VnPay:HashSecret"];
             vnp_TmnCode = _configuration["VnPay:TmnCode"];
        }

        private readonly string vnp_HashSecret;
        private readonly string vnp_TmnCode;
        public string CreatePaymentUrl(Booking booking, string returnUrl)
        {
            //var orderType = 250000;
            //string data = $"{order.OrderId}|2.1.0|pay|{vnp_TmnCode}|{order.OrderId}|{order.OrderDate:yyyyMMddHHmmss}|{order.OrderDate:yyyyMMddHHmmss}|{"127.0.0.1"}|Thanh toan don hang: {order.OrderId}";

            //var ExpireDate = order.OrderDate.AddMinutes(15).ToString("yyyyMMddHHmmss");
            //string checksum = Utils.HmacSHA512(vnp_HashSecret, data);
            //decimal convertMoney = order.TotalAmount * 25000;
            returnUrl = "https://google.com.vn";

            var vnPay = new VnPayLibrary();
            
            vnPay.AddRequestData("vnp_Amount", ((int)booking.TotalPrice * 100000).ToString());
            vnPay.AddRequestData("vnp_Command", "pay");
            vnPay.AddRequestData("vnp_CreateDate", booking.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", "VND");
            vnPay.AddRequestData("vnp_IpAddr", "127.0.0.1");
            vnPay.AddRequestData("vnp_Locale", "vn");
            vnPay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang: {booking.Id}");
            vnPay.AddRequestData("vnp_OrderType", "other");
            vnPay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnPay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnPay.AddRequestData("vnp_TxnRef", booking.Id.ToString());
            vnPay.AddRequestData("vnp_Version", "2.1.0");
            /*vnPay.AddRequestData("vnp_Amount", convertMoney.ToString());*/

            string paymentUrl = vnPay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;

        }
        public string CreateDepositPayment(int amount, string returnUrl)
        {
            returnUrl = "https://localhost:7262/api/Payment/PaymentReturn-VNPAY";

            var vnPay = new VnPayLibrary();

            vnPay.AddRequestData("vnp_Amount", ((amount*250000).ToString()));     //xử lí cái này
            vnPay.AddRequestData("vnp_Command", "pay");
            vnPay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", "VND");
            vnPay.AddRequestData("vnp_IpAddr", "127.0.0.1");
            vnPay.AddRequestData("vnp_Locale", "vn");
            vnPay.AddRequestData("vnp_OrderInfo", $"Xu ly nap tien");
            vnPay.AddRequestData("vnp_OrderType", "other");
            vnPay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnPay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnPay.AddRequestData("vnp_TxnRef", "Deposit");
            vnPay.AddRequestData("vnp_Version", "2.1.0");
            /*vnPay.AddRequestData("vnp_Amount", convertMoney.ToString());*/

            string paymentUrl = vnPay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }
        public string CreatePayment(Booking booking, string returnUrl)
        {
            //var orderType = 250000;
            //string data = $"{order.OrderId}|2.1.0|pay|{vnp_TmnCode}|{order.OrderId}|{order.OrderDate:yyyyMMddHHmmss}|{order.OrderDate:yyyyMMddHHmmss}|{"127.0.0.1"}|Thanh toan don hang: {order.OrderId}";

            //var ExpireDate = order.OrderDate.AddMinutes(15).ToString("yyyyMMddHHmmss");
            //string checksum = Utils.HmacSHA512(vnp_HashSecret, data);
            //decimal convertMoney = order.TotalAmount * 25000;
            returnUrl = "https://localhost:7262/api/Payment/PaymentReturn-VNPAY";

            var vnPay = new VnPayLibrary();

            vnPay.AddRequestData("vnp_Amount", ((int)booking.TotalPrice*250000).ToString());     //xử lí cái này
            vnPay.AddRequestData("vnp_Command", "pay");
            vnPay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", "VND");
            vnPay.AddRequestData("vnp_IpAddr", "127.0.0.1");
            vnPay.AddRequestData("vnp_Locale", "vn");
            vnPay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang: {booking.Id}");
            vnPay.AddRequestData("vnp_OrderType", "other");
            vnPay.AddRequestData("vnp_ReturnUrl", returnUrl);
            vnPay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnPay.AddRequestData("vnp_TxnRef", booking.Id.ToString());
            vnPay.AddRequestData("vnp_Version", "2.1.0");
            /*vnPay.AddRequestData("vnp_Amount", convertMoney.ToString());*/

            string paymentUrl = vnPay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;

        }
        public bool ValidateSignature(string queryString, string vnp_HashSecret)
        {
            var vnPay = new VnPayLibrary();

            // check signature
            return vnPay.ValidateSignature(queryString, vnp_HashSecret);
        }

        public async Task<HttpResponseMessage> SendRefundRequestAsync(VnpayRefundRequest request, string url)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                return await client.PostAsync(url, content);
            }
        }


        public string GenerateSecureHash(VnpayRefundRequest request, string secretKey)
        {
            string data = $"{request.vnp_RequestId}|{request.vnp_Version}|{request.vnp_Command}|{request.vnp_TmnCode}|{request.vnp_TransactionType}|{request.vnp_TxnRef}|{request.vnp_Amount}|{request.vnp_TransactionNo}|{request.vnp_TransactionDate}|{request.vnp_CreateBy}|{request.vnp_CreateDate}|{request.vnp_IpAddr}|{request.vnp_OrderInfo}";
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey)))
            {
                byte[] hashBytes = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }


        private Dictionary<string, string> ParseResponse(string response)
        {
            return response.Split('&')
                           .Select(part => part.Split('='))
                           .ToDictionary(split => split[0], split => split[1]);
        }

        public class VnpayRefundRequest
        {
            public string vnp_RequestId { get; set; }
            public string vnp_Version { get; set; } = "2.1.0";
            public string vnp_Command { get; set; } = "refund";
            public string vnp_TmnCode { get; set; }
            public string vnp_TransactionType { get; set; }
            public string vnp_TxnRef { get; set; }
            public long vnp_Amount { get; set; }
            public string vnp_OrderInfo { get; set; }
            public string vnp_TransactionNo { get; set; }
            public string vnp_TransactionDate { get; set; }
            public string vnp_CreateBy { get; set; }
            public string vnp_CreateDate { get; set; }
            public string vnp_IpAddr { get; set; }
            public string vnp_SecureHash { get; set; }
        }
    }
}
