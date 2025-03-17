using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCP.DTOs.DTOs.PaymentDTO
{
    public class PaymentBodyDTO
    {
        public Guid Id { get; set; }
        [JsonPropertyName("paymentMethod")]
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string? Uid { get; set; }
    }
}
