using HCP.Service.DTOs.BookingDTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.RatingDTO
{
    public class RatingDTO
    {
        public class CreateRatingRequestDTO
        {
            [JsonPropertyName("rating")]
            [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
            public decimal Rating { get; set; }

            [JsonPropertyName("review")]
            public string Review { get; set; } = "No Comment";

            [JsonPropertyName("cleaning-service-id")]
            public Guid CleaningServiceId { get; set; }

            [JsonPropertyName("booking-id")]
            public Guid BookingId { get; set; }
        }

        public class CreatedRatingResponseDTO
        {
            [JsonPropertyName("customer-id")]
            public string CustomerId { get; set; }

            [JsonPropertyName("service-name")]
            public string ServiceName { get; set; }

            [JsonPropertyName("rating-id")]
            public Guid RatingId { get; set; }

            [JsonPropertyName("cleaning-service-id")]
            public Guid CleaningServiceId { get; set; }

            [JsonPropertyName("booking-id")]
            public Guid BookingId { get; set; }

            [JsonPropertyName("rating")]
            [JsonConverter(typeof(SingleDecimalPlaceConverter))]
            public decimal Rating { get; set; }

            [JsonPropertyName("review")]
            public string Review { get; set; }

            [JsonPropertyName("rating-date")]
            public DateTime RatingDate { get; set; }

            [JsonPropertyName("customer-name")]
            public string CustomerName { get; set; }

            [JsonPropertyName("customer-avatar")]
            public string CustomerAvatar { get; set; }
        }

        public class RatingResponseListDTO
        {
            [JsonPropertyName("service-name")]
            public string ServiceName { get; set; }

            [JsonPropertyName("rating")]
            public decimal Rating { get; set; }

            [JsonPropertyName("review")]
            public string Review { get; set; }

            [JsonPropertyName("customer-name")]
            public string CustomerName { get; set; }

            [JsonPropertyName("customer-avatar")]
            public string CustomerAvatar { get; set; }

            [JsonPropertyName("rating-date")]
            public DateTime RatingDate { get; set; }
        }

        public class PagingRatingResponseListDTO
        {
            [JsonPropertyName("ratings")]
            public List<RatingResponseListDTO> Items { get; set; }

            [JsonPropertyName("rating-average")]
            [JsonConverter(typeof(SingleDecimalPlaceConverter))]
            public decimal RatingAvg { get; set; }

            [JsonPropertyName("total-count")]
            public int TotalCount { get; set; }

            [JsonPropertyName("total-page")]
            public int TotalPages { get; set; }

            [JsonPropertyName("has-next")]
            public bool HasNext { get; set; }

            [JsonPropertyName("has-previous")]
            public bool HasPrevious { get; set; }
        }

        public class HousekeperRatingDTO
        {
            [JsonPropertyName("housekeeper-id")]
            public string HousekeeperId { get; set; }
            
            [JsonPropertyName("housekeeper-name")]
            public string HousekeeperName { get; set; }

            [JsonPropertyName("housekeeper-avatar")]
            public string CustomerAvatar { get; set; }

            [JsonPropertyName("rating")]
            [JsonConverter(typeof(SingleDecimalPlaceConverter))]
            public decimal Rating { get; set; }

            [JsonPropertyName("total-rating")]
            public decimal TotalRating { get; set; }
        }

        public class SingleDecimalPlaceConverter : JsonConverter<decimal>
        {
            public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return reader.GetDecimal();
            }

            public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
            {
                writer.WriteNumberValue(Math.Round(value, 1));
            }
        }
    }
}
