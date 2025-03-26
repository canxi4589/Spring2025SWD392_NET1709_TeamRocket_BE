using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HCP.Repository.Entities;
using Microsoft.Extensions.Configuration;
namespace HCP.Service.Services;

public class TokenHelper : ITokenHelper
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppUser> _userManager;
    public TokenHelper(IConfiguration configuration, UserManager<AppUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }

    public string GenerateJwtToken(AppUser user, string role)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);
        var phoneNumber = user.PhoneNumber ?? "0366532672";
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim("id", user.Id),
            new Claim("role", role),
            new Claim("email",user.Email!),
            new Claim("phone",phoneNumber),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        var key = new SymmetricSecurityKey(secretKey);
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["AccessTokenExpirationMinutes"])),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public async Task<string> GenerateRefreshToken(AppUser user)
    {
        var newRefreshToken = Guid.NewGuid().ToString();
        await _userManager.SetAuthenticationTokenAsync(user, "MyApp", "RefreshToken", newRefreshToken);
        return newRefreshToken;
    }
    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"])), 
            ValidateIssuer = false,  
            ValidateAudience = false, 
            ValidateLifetime = false, 
            ClockSkew = TimeSpan.Zero
        };
        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }
            return principal;
        }
        catch
        {
            return null;
        }
    }

}
