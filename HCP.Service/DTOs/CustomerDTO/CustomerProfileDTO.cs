using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.CustomerDTO
{
    public class CustomerProfileDTO
    {
        [JsonPropertyName("full_name")]
        public string? FullName { get; set; }

        [JsonPropertyName("birth_date")]
        public DateTime? Birthday { get; set; }

        [JsonPropertyName("gender")]
        public bool? Gender { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("phone")]
        public string? PhoneNumber { get; set; }
    }

    public class UpdateCusProfileDto
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full Name must contain only letters and spaces.")]
        [JsonPropertyName("full_name")]
        public required string FullName { get; set; }

        [Required]
        [JsonPropertyName("gender")]
        public bool Gender { get; set; }

        [Required]
        [Phone]
        [JsonPropertyName("phone")]
        public required string PhoneNumber { get; set; }

        [Required]
        [JsonPropertyName("birth_date")]
        public required DateTime Birthdate { get; set; }
    }
}
