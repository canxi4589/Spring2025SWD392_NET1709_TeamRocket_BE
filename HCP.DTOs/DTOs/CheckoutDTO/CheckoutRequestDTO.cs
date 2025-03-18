using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCP.DTOs.DTOs.CheckoutDTO
{
    public class CheckoutRequestDTO1
    {
        [JsonPropertyName("service_id")]
        public Guid ServiceId { get; set; }

        [JsonPropertyName("address_id")]
        public Guid AddressId { get; set; }

        [JsonPropertyName("timeslot_id")]
        public Guid ServiceTimeSlotId { get; set; }

        [JsonPropertyName("booking_date")]
        public DateTime BookingDate { get; set; }

        [JsonPropertyName("additional_services")]
        public List<CheckoutAdditionalServiceRequestDTO> AdditionalServices { get; set; } = new();
    }

    public class CheckoutAdditionalServiceRequestDTO
    {
        [JsonPropertyName("additional_service_id")]
        public Guid AdditionalServiceId { get; set; }
    }
}
