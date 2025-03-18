using HCP.Repository.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCP.DTOs.DTOs.HousekeeperDTOs
{
    public class HousekeeperRegisterRequestDTO
    {
        [Required]
        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [Required]
        [Phone]
        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        [JsonPropertyName("confirm_password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [JsonPropertyName("certificate_picture")]
        public string Pdf { get; set; }

        [Required]
        [JsonPropertyName("id_card_front")]
        public string IdCardFront { get; set; }

        [Required]
        [JsonPropertyName("id_card_back")]
        public string IdCardBack { get; set; }

        [JsonPropertyName("housekeeper_categories")]
        public List<Guid> HousekeeperCategories { get; set; }

        [MaxLength(255)]
        [JsonPropertyName("housekeeper_address_line")]
        public string AddressLine1 { get; set; }

        [Required]
        [MaxLength(100)]
        [JsonPropertyName("housekeeper_city")]
        public string City { get; set; }

        [Required]
        [JsonPropertyName("housekeeper_place_id")]
        public string PlaceId { get; set; }

        [Required]
        [MaxLength(100)]
        [JsonPropertyName("housekeeper_district")]
        public string District { get; set; }

        [MaxLength(50)]
        [JsonPropertyName("housekeeper_address_title")]
        public string? Title { get; set; }
    }
}