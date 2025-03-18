using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCP.DTOs.DTOs.HousekeeperDTOs
{
    public class HousekeeperProfileDTO
    {
        [JsonPropertyName("housekeeper_id")]
        public string HousekeeperId { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Phone]
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("certificate_picture")]
        public string Pdf { get; set; }

        [JsonPropertyName("id_card_front")]
        public string IdCardFront { get; set; }

        [JsonPropertyName("id_card_back")]
        public string IdCardBack { get; set; }

        [JsonPropertyName("housekeeper_categories")]
        public List<Guid> HousekeeperCategories { get; set; }

        [JsonPropertyName("housekeeper_address_line")]
        public string AddressLine1 { get; set; }

        [JsonPropertyName("housekeeper_city")]
        public string City { get; set; }

        [JsonPropertyName("housekeeper_place_id")]
        public string PlaceId { get; set; }

        [JsonPropertyName("housekeeper_district")]
        public string District { get; set; }

        [JsonPropertyName("housekeeper_address_title")]
        public string? Title { get; set; }

        [JsonPropertyName("housekeeper_status")]
        public string Status { get; set; }

        [JsonPropertyName("approved_by")]
        public string ApprovedBy { get; set; } = null!;
    }
}
