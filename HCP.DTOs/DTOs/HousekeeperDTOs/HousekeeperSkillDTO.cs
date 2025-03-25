using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCP.DTOs.DTOs.HousekeeperDTOs
{
    public class HousekeeperSkillDTO
    {
        [JsonPropertyName("category_id")]
        public Guid CategoryId { get; set; }

        [JsonPropertyName("category_name")]
        public string? CategoryName { get; set; }
    }
}
