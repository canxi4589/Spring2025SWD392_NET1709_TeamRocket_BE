using HCP.Service.DTOs.HousekeeperDTOs;

namespace HCP.Service.Services.AuthenticationService
{
    public interface IAuthenticationService
    {
        Task<HousekeeperRegisterResponseDTO?> HousekeeperRegister(HousekeeperRegisterRequestDTO requestDTO);
    }
}