using HCP.Repository.Entities;
using System.Security.Claims;
namespace HCP.Service.Services;

public interface ITokenHelper
{
    string GenerateJwtToken(AppUser user, string role);
    Task<string> GenerateRefreshToken(AppUser user);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}