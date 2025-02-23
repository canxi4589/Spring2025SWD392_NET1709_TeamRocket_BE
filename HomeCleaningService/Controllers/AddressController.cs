using HCP.Repository.Entities;
using HCP.Service.DTOs.CustomerDTO;
using HCP.Service.Services.CustomerService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly UserManager<AppUser> _userManager;

        public AddressController(IAddressService addressService, UserManager<AppUser> userManager)
        {
            _addressService = addressService;
            _userManager = userManager;
        }

        [HttpGet("Customer")]
        public async Task<IActionResult> getAddressByCustomerId(string mail)
        {
            var user = _userManager.FindByEmailAsync(mail);
            if (user != null)
            {
                var list = await _addressService.GetAddressByUser(mail);
                return Ok(new AppResponse<List<AddressDTO>>().SetSuccessResponse(list));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse("Error", "User not found"));
        }
    }
}
