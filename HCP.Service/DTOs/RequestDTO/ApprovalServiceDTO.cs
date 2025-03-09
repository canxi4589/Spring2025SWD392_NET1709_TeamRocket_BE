using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.RequestDTO
{
    public class ApprovalServiceDTO
    {
        [JsonPropertyName("service_id")]
        public Guid ServiceId { get; set; }

        [JsonPropertyName("service_name")]
        public string ServiceName { get; set; }

        [JsonPropertyName("category_id")]
        public Guid CategoryId { get; set; }

        [JsonPropertyName("category_name")]
        public string CategoryName { get; set; }

        [JsonPropertyName("category_picture_url")]
        public string PictureUrl { get; set; }

        [JsonPropertyName("service_description")]
        public string Description { get; set; }

        [JsonPropertyName("service_status")]
        public string Status { get; set; }

        [JsonPropertyName("service_price")]
        public decimal Price { get; set; }

        [JsonPropertyName("service_city")]
        public string City { get; set; }

        [JsonPropertyName("service_district")]
        public string District { get; set; }

        [JsonPropertyName("service_place_id")]
        public string PlaceId { get; set; }

        [JsonPropertyName("service_address")]
        public string AddressLine { get; set; }

        [JsonPropertyName("duration")]
        public double Duration { get; set; }

        [JsonPropertyName("created_time")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updated_time")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonPropertyName("user_name")]
        public string UserName { get; set; }

        public List<AdditionalServiceDTO> AdditionalServices { get; set; } = new();
        public List<ServiceImgDTO> ServiceImages { get; set; } = new();
        public List<ServiceStepsDTO> ServiceSteps { get; set; } = new();
        public List<ServiceTimeSlotDTO> ServiceTimeSlots { get; set; } = new();
        public List<DistanceRuleDTO> ServiceDistanceRule { get; set; } = new();
    }
}
