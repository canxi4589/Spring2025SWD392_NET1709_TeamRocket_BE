using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HCP.Service.DTOs.CheckoutDTO
{
    public class CheckoutResponseDTO1
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [JsonPropertyName("checkout_id")]
        public Guid CheckoutId { get; set; }

        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; }

        [JsonPropertyName("service_id")]
        public Guid CleaningServiceId { get; set; }

        [JsonPropertyName("service_name")]
        public string CleaningServiceName { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("base_service_price")]
        public decimal ServicePrice { get; set; }

        [JsonPropertyName("additional_price")]
        public decimal AdditionalPrice { get; set; }

        [JsonPropertyName("distance")]
        public string Distance { get; set; } // Example: "2.5 km"

        [JsonPropertyName("distance_price")]
        public decimal DistancePrice { get; set; }

        [JsonPropertyName("total_price")]
        public decimal TotalPrice { get; set; }

        [JsonPropertyName("address_id")]
        public Guid AddressId { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("district")]
        public string District { get; set; }

        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; }

        [JsonPropertyName("address")]
        public string AddressLine { get; set; }

        [JsonPropertyName("time_slot_id")]
        public Guid TimeSlotId { get; set; }

        [JsonPropertyName("booking_date")]
        public DateTime BookingDate { get; set; }

        [JsonPropertyName("start_time")]
        public TimeSpan StartTime { get; set; }

        [JsonPropertyName("end_time")]
        public TimeSpan EndTime { get; set; }

        [JsonPropertyName("date_of_week")]
        public string DateOfWeek { get; set; }

        [JsonPropertyName("additional_services")]
        public List<CheckoutAdditionalServiceResponseDTO> AdditionalServices { get; set; } = new();

        [JsonPropertyName("payment_methods")]
        public List<PaymentMethodDTO> PaymentMethods { get; set; } = new();
        public string? PaymentMethod {  get; set; }
    }

    public class CheckoutAdditionalServiceResponseDTO
    {
        [JsonPropertyName("additional_service_id")]
        public Guid AdditionalServiceId { get; set; }

        [JsonPropertyName("additional_service_name")]
        public string AdditionalServiceName { get; set; }

        [JsonPropertyName("additional_service_price")]
        public decimal Amount { get; set; }

        [JsonPropertyName("status")]
        public bool IsActive { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("duration")]
        public double? Duration { get; set; }
    }

    public class PaymentMethodDTO
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("is_choosable")]
        public bool IsChoosable { get; set; }
    }
}
