using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.RequestDTO
{
    public class PendingRequestDTO
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

    public class AdditionalServiceDTO
    {
        [JsonPropertyName("additional_service_name")]
        public string Name { get; set; } = "This is Name";

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("duration")]
        public double? Duration { get; set; }
    }

    public class ServiceImgDTO
    {
        [JsonPropertyName("link")]
        public string LinkUrl { get; set; } = "https://picsum.photos/seed/picsum/200/300";
    }

    public class ServiceStepsDTO
    {
        [JsonPropertyName("step_order")]
        public int StepOrder { get; set; }

        [JsonPropertyName("step_description")]
        public string StepDescription { get; set; } = "This is Step Description";
    }

    public class ServiceTimeSlotDTO
    {
        [JsonPropertyName("start_time")]
        public TimeSpan StartTime { get; set; }

        [JsonPropertyName("end_time")]
        public TimeSpan EndTime { get; set; }

        [JsonPropertyName("day_of_week")]
        public string DayOfWeek { get; set; }
    }

    public class DistanceRuleDTO
    {
        [JsonPropertyName("min_distance")]
        public double MinDistance { get; set; }

        [JsonPropertyName("max_distance")]
        public double MaxDistance { get; set; }

        [JsonPropertyName("base_fee")]
        public decimal BaseFee { get; set; }

        [JsonPropertyName("extra_per_km")]
        public decimal ExtraPerKm { get; set; }
    }
}