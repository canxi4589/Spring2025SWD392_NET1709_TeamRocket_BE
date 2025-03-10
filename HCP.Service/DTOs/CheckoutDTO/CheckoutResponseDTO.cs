using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.CheckoutDTO
{
    public class CheckoutResponseDTO1
    {
        [JsonPropertyName("checkout_id")]
        public Guid CheckoutId { get; set; }

        [JsonPropertyName("customer_id")]
        public string CustomerId { get; set; }

        [JsonPropertyName("service_id")]
        public Guid CleaningServiceId { get; set; }

        [JsonPropertyName("service_name")]
        public string CleaningServiceName { get; set; } = "This is a really good Cleaning Service Name";

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("base_service_price")]
        public decimal ServicePrice { get; set; }

        [JsonPropertyName("addtional_price")]
        public decimal AdditionalPrice { get; set; }

        [JsonPropertyName("address_id")]
        public Guid AddressId {  get; set; }

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

        [JsonPropertyName("start_time")]
        public TimeSpan StartTime { get; set; }

        [JsonPropertyName("end_time")]
        public TimeSpan EndTime { get; set; }

        [JsonPropertyName("date_of_week")]
        public string DateOfWeek { get; set; }

        [JsonPropertyName("total_price")]
        public decimal TotalPrice {  get; set; }

        [JsonPropertyName("distance_price")]
        public decimal DistancePrice {  get; set; }

        // List of additional services
        [JsonPropertyName("additional_services")]
        public List<CheckoutAdditionalServiceResponseDTO> AdditionalServices { get; set; } = new ();
    }

    public class CheckoutAdditionalServiceResponseDTO
    {
        [JsonPropertyName("addtional_service_id")]
        public Guid AdditionalServiceId { get; set; }

        [JsonPropertyName("addtional_service_name")]
        public string AdditionalServiceName { get; set; } = "This is a cool Name";
        
        [JsonPropertyName("additinal_service_price")]
        public double Amount { get; set; }

        [JsonPropertyName("status")]
        public bool IsActive { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("duration")]
        public double? Duration { get; set; }
    }
}
