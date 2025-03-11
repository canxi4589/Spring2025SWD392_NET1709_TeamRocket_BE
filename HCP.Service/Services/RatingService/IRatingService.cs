using HCP.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static HCP.Service.DTOs.RatingDTO.RatingDTO;

namespace HCP.Service.Services.RatingService
{
    public interface IServiceRatingService
    {
        Task<CreatedRatingResponseDTO> CreateRating(CreateRatingRequestDTO request, ClaimsPrincipal customer);
        Task<RatingResponseDTO> GetRatingsByCustomer(string userId);
        Task<RatingResponseDTO> GetRatingsByService(Guid serviceId);
        Task<RatingResponseDTO> SortRatings(Guid serviceId, decimal minRating, decimal maxRating);
    }
}
