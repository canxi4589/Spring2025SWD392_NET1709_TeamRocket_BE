using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.RatingDTO
{
    public class RatingDTO
    {
        public class CreateRatingRequestDTO
        {
            [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
            public decimal Rating { get; set; }

            public string Review { get; set; } = "No Comment";

            public Guid CleaningServiceId { get; set; }
        }
        public class CreatedRatingResponseDTO
        {
            public string CustomerId { get; set; }
            public Guid RatingId { get; set; }
            public Guid CleaningServiceId { get; set; }
            public decimal Rating { get; set; }
            public string Review { get; set; }
            public string CustomerName { get; set; }
            public string CustomerAvatar { get; set; }
        }

        public class RatingResponseDTO
        {
            public int RatingCount { get; set; }

            public List<RatingResponseListDTO> Ratings { get; set; }

        }

        public class RatingResponseListDTO
        {
            public decimal Rating { get; set; }
            public string Review { get; set; }
            public string CustomerName { get; set; }
            public string CustomerAvatar { get; set; }
        }

    }
}
