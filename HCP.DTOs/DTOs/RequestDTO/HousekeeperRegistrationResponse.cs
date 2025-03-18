using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static HCP.DTOs.DTOs.RatingDTO.RatingDTO;

namespace HCP.DTOs.DTOs.RequestDTO
{
    public class RegistrationRequestDetailDTO
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

        [MaxLength(50)]
        [JsonPropertyName("housekeeper_status")]
        public string? Status { get; set; }
    }

    public class RegistrationRequestDTO
    {
        [JsonPropertyName("housekeeper_id")]
        public string HousekeeperId { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("categories")]
        public List<Guid> HousekeeperCatgories { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public class RegistrationRequestListDTO
    {
        [JsonPropertyName("registration_requests")]
        public List<RegistrationRequestDTO> Items { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("total_page")]
        public int TotalPages { get; set; }

        [JsonPropertyName("has_next")]
        public bool HasNext { get; set; }

        [JsonPropertyName("has_previous")]
        public bool HasPrevious { get; set; }
    }

    public class RegistrationRequestStatusUpdateDto
    {
        [JsonPropertyName("housekeeper_id")]
        public string HousekeeperId { get; set; }

        [JsonPropertyName("is_approve")]
        public bool IsApprove { get; set; }   // True = "active", False = "rejected"

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }
    }

}
