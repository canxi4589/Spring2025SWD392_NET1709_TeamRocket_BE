﻿using System.Security.Claims;
using HCP.DTOs.DTOs.CustomerDTO;
using HCP.Repository.Constance;
using HCP.Repository.Entities;
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

        //[HttpGet()]
        //[Authorize]
        //public async Task<IActionResult> getAddressByCustomer()
        //{
        //    var user = await _customerService.GetCustomerAsync(User);
        //    if (user != null)
        //    {
        //        var list = await _addressService.GetAddressByUser(user);
        //        return Ok(new AppResponse<List<AddressDTO>>().SetSuccessResponse(list));
        //    }
        //    return NotFound(new AppResponse<string>().SetErrorResponse("Error", "User not found"));
        //}

        [HttpGet()]
        [Authorize]
        public async Task<IActionResult> getAddressByCustomerPaging(int? pageIndex, int? pageSize)
        {
            var user = await _customerService.GetCustomerAsync(User);
            if (user != null)
            {
                var list = await _addressService.GetAddressByUserPaging(user, pageIndex, pageSize);
                return Ok(new AppResponse<GetAddressListDTO>().SetSuccessResponse(list));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse(KeyConst.NotFound, CommonConst.NotFoundError));
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
                return BadRequest(new AppResponse<string>().SetErrorResponse(KeyConst.Error, CommonConst.SomethingWrongMessage));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse(KeyConst.NotFound, CommonConst.NotFoundError));
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
                return BadRequest(new AppResponse<string>().SetErrorResponse(KeyConst.Error, CommonConst.SomethingWrongMessage));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse(KeyConst.NotFound, CommonConst.NotFoundError));
        }
        [HttpDelete()]
        [Authorize]
        public async Task<IActionResult> deleteAddress(Guid addressId)
        {
            var user = await _customerService.GetCustomerAsync(User);
            if (user != null)
            {
                var address = await _addressService.DeleteAddress(user, addressId);
                if (address == true)
                {
                    var successResponse = new AppResponse<bool>().SetSuccessResponse(address);
                    return Ok(successResponse);
                }
                return BadRequest(new AppResponse<string>().SetErrorResponse(KeyConst.Error, CommonConst.SomethingWrongMessage));
            }
            return NotFound(new AppResponse<string>().SetErrorResponse(KeyConst.Error, CommonConst.SomethingWrongMessage));
        }

    }
}
