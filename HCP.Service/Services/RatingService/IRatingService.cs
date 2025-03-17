using System.Security.Claims;
using static HCP.DTOs.DTOs.RatingDTO.RatingDTO;

namespace HCP.Service.Services.RatingService
{
    public interface IRatingService
    {
        Task<CreatedRatingResponseDTO> CreateRating(CreateRatingRequestDTO request, ClaimsPrincipal customer);
        Task<PagingRatingResponseListDTO> GetRatingsByCustomer(ClaimsPrincipal user, int? pageIndex, int? pageSize);
        Task<PagingRatingResponseListDTO> GetRatingsByService(Guid serviceId, int? pageIndex, int? pageSize);
        Task<PagingRatingResponseListDTO> SortRatings(Guid serviceId, decimal rate, int? pageIndex, int? pageSize);
        Task<HousekeperRatingDTO> GetHousekeeperRatingAsync(string userId);
    }
}
