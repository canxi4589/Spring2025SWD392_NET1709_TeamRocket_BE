using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HCP.Service.Integrations.Currency
{
    public static class CurrencyService
    {
        private static readonly string ExchangeRateURL =
            "https://portal.vietcombank.com.vn/Usercontrols/TVPortal.TyGia/pXML.aspx?b=10";

        public static async Task<int?> ConvertToVNDFrom(
            this double amount,
            string currencyCode,
            RateType rateType = RateType.Buy
        )
        {
            var exchangeRate = await GetRateOf(currencyCode);
            return exchangeRate == null
                ? null
                : rateType switch
                {
                    RateType.Buy => Convert.ToInt32(exchangeRate.BuyRate * (decimal)amount),
                    RateType.Transfer => Convert.ToInt32(
                        exchangeRate.TransferRate * (decimal)amount
                    ),
                    RateType.Sell => Convert.ToInt32(exchangeRate.SellRate * (decimal)amount),
                    _ => throw new ArgumentOutOfRangeException(
                        nameof(rateType),
                        "Invalid rate type"
                    ),
                };
        }

        private static async Task<ExchangeRate?> GetRateOf(string currencyCode)
        {
            using var client = new HttpClient();
            var response = await client.GetStringAsync(ExchangeRateURL);
            if (string.IsNullOrWhiteSpace(response) || !response.Contains("<ExrateList>"))
                throw new Exception("Invalid response from Vietcombank");
            return ParseRate(response, currencyCode);
        }

        private static ExchangeRate? ParseRate(string xml, string currencyCode)
        {
            var doc = XDocument.Parse(xml);
            return (
                from r in doc.Descendants("Exrate")
                where r.Attribute("CurrencyCode")?.Value == currencyCode
                select new ExchangeRate
                {
                    CurrencyCode = r.Attribute("CurrencyCode")?.Value!,
                    CurrencyName = r.Attribute("CurrencyName")?.Value.Trim()!,
                    BuyRate = decimal.Parse(r.Attribute("Buy")?.Value.Replace(",", "")!),
                    TransferRate = decimal.Parse(r.Attribute("Transfer")?.Value.Replace(",", "")!),
                    SellRate = decimal.Parse(r.Attribute("Sell")?.Value.Replace(",", "")!),
                }
            ).FirstOrDefault();
        }

        private class ExchangeRate
        {
            public string CurrencyCode { get; set; }
            public string CurrencyName { get; set; }
            public decimal BuyRate { get; set; }
            public decimal TransferRate { get; set; }
            public decimal SellRate { get; set; }
        }

        public enum RateType
        {
            Buy,
            Transfer,
            Sell,
        }
    }
}
