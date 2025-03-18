using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCP.DTOs.DTOs.HousekeeperDTOs
{
    public class HousekeeperRegisterResponseDTO
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

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("confirm_password")]
        public string ConfirmPassword { get; set; }

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

        [MaxLength(255)]
        [JsonPropertyName("housekeeper_address_line")]
        public string AddressLine1 { get; set; }

        [MaxLength(100)]
        [JsonPropertyName("housekeeper_city")]
        public string City { get; set; }

        [JsonPropertyName("housekeeper_place_id")]
        public string PlaceId { get; set; }

        [MaxLength(100)]
        [JsonPropertyName("housekeeper_district")]
        public string District { get; set; }

        [MaxLength(50)]
        [JsonPropertyName("housekeeper_address_title")]
        public string? Title { get; set; }
    }
}

