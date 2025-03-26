using HCP.DTOs.DTOs.HousekeeperDTOs;
using System.Security.Claims;

namespace HCP.Service.Services.HousekeeperService
{
    public interface IHousekeeperService
    {
        Task<HousekeeperProfileDTO> GetHousekeeperProfile(ClaimsPrincipal currentHousekeeper);
        Task<HousekeeperEarningListDTO> GetHousekeeperEarnings(ClaimsPrincipal housekeeperClaim, int? pageIndex, int? pageSize, int? day, int? month, int? year);
    }
}