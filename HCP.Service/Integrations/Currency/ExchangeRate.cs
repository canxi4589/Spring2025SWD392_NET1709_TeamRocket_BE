using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HCP.Service.Integrations.Currency
{
    public class ExchangRate
    {
        public string? ExchangeRateUrl { get; set; } =
            "https://portal.vietcombank.com.vn/Usercontrols/TVPortal.TyGia/pXML.aspx?b=10";

        public ExchangRate() { }

        public ExchangRate(IConfiguration configuration)
        {
            ExchangeRateUrl = configuration["VietComBank:ExchangeRateUrl"] ?? "";
        }

        public async Task<double> GetUsdToVndExchangeRateAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(ExchangeRateUrl);
                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    return ParseExchangeRate(responseData);
                }
                throw new Exception("Unable to get exchange rate.");
            }
        }

        private static double ParseExchangeRate(string xmlData)
        {
            XDocument doc = XDocument.Parse(xmlData);
            // Tìm tỷ giá USD
            var usdRateElement = doc
                .Root?.Elements("Exrate")
                .FirstOrDefault(x => (string)x.Attribute("CurrencyCode") == "USD");

            if (usdRateElement != null)
            {
                // Trả về tỷ giá mua
                return double.Parse(
                    (string)usdRateElement.Attribute("Buy"),
                    CultureInfo.InvariantCulture
                );
            }
            throw new Exception("USD exchange rate not found.");
        }

        public double ConvertUsdToVnd(double amountInUsd, double exchangeRate)
        {
            return amountInUsd * exchangeRate;
        }
    }
}
