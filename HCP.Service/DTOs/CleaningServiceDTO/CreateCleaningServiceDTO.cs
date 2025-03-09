using Microsoft.AspNetCore.Routing.Constraints;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HCP.Service.DTOs.CleaningServiceDTO
{
    public class CreateCleaningServiceDTO
    {
        [Required]
        [JsonPropertyName("service_name")]
        public string ServiceName { get; set; } = "This is Service Name";

        [Required]
        [JsonPropertyName("category_id")]
        public Guid CategoryId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = "This is Description";

        [Required]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [Required]
        [JsonPropertyName("city")]
        public string City { get; set; } = "Raumania";

        [Required]
        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; } = "Raumania";

        [Required]
        [JsonPropertyName("district")]
        public string District { get; set; } = "Binh Thanh";

        [Required]
        [JsonPropertyName("address_line")]
        public string AddressLine { get; set; } = "This is Address Line";

        [Required]
        [JsonPropertyName("duration")]
        public double Duration { get; set; }

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
        public string Description { get; set; } = "This is a Cleaning Service";

        [JsonPropertyName("duration")]
        public double Duration { get; set; }
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