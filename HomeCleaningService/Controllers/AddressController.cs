using System.Security.Claims;
using HCP.Repository.Entities;
using HCP.Service.DTOs.CustomerDTO;
using HCP.Service.Services.CustomerService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
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
        private readonly ICustomerService _customerService;

        public AddressController(IAddressService addressService, UserManager<AppUser> userManager, ICustomerService customerService)
        {
            _addressService = addressService;
            _userManager = userManager;
            _customerService = customerService;
        }

        [HttpGet()]
        [Authorize]
        public async Task<IActionResult> getAddressByCustomer()
        {
            var user = await _customerService.GetCustomerAsync(User);
            if (user != null)
            {
                var list = await _addressService.GetAddressByUser(user);
                return Ok(new AppResponse<List<AddressDTO>>().SetSuccessResponse(list));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse("Error", "User not found"));
        }
        [HttpPost()]
        [Authorize]
        public async Task<IActionResult> createAddress(CreataAddressDTO creataAddressDTO)
        {
            var user = await _customerService.GetCustomerAsync(User);
            if(user != null)
            {
                var address = await _addressService.CreateAddress(user, creataAddressDTO);
                if (address != null)
                {
                    var successResponse = new AppResponse<AddressDTO>().SetSuccessResponse(address);
                    return Ok(successResponse);
                }
                return BadRequest(new AppResponse<string>().SetErrorResponse("Error", "Failed to create address"));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse("Error", "User not found"));
        }
        [HttpPut()]
        [Authorize]
        public async Task<IActionResult> updateAddress(UpdateAddressDTO updateAddressDTO)
        {
            var user = await _customerService.GetCustomerAsync(User);
            if (user != null)
            {
                var address = await _addressService.UpdateAddress(user, updateAddressDTO);
                if (address != null)
                {
                    var successResponse = new AppResponse<AddressDTO>().SetSuccessResponse(address);
                    return Ok(successResponse);
                }
                return BadRequest(new AppResponse<string>().SetErrorResponse("Error", "Failed to update address"));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse("Error", "User not found"));
        }
    }
}
