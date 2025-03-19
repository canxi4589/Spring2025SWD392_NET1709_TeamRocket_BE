using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCP.DTOs.DTOs.AdminManagementDTO
{
    public class CreateStaffRequestDTO
    {
        [Required]
        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

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
    }

    public class CreateStaffResponseDTO
    {
        [JsonPropertyName("staff_id")]
        public string StaffId { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("hash_password")]
        public string HashPassword { get; set; }
    }
}
