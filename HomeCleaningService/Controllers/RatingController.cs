using HCP.Service.Services.RatingService;
using HCP.Service.DTOs.RatingDTO;
using HCP.Repository.Constance;
using HCP.Repository.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Threading.Tasks;
using HomeCleaningService.Helpers;
using static HCP.Service.DTOs.RatingDTO.RatingDTO;
using Azure;
using Microsoft.IdentityModel.Tokens;

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateRating([FromBody] CreateRatingRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AppResponse<CreatedRatingResponseDTO>().SetErrorResponse(KeyConst.Rating, CommonConst.SomethingWrongMessage));

            try
            {
                var result = await _ratingService.CreateRating(request, User);
                return Ok(new AppResponse<CreatedRatingResponseDTO>().SetSuccessResponse(result));
            }
            catch (Exception ex)
            {
                return BadRequest(new AppResponse<CreatedRatingResponseDTO>().SetErrorResponse(KeyConst.Rating, ex.Message));
            }
        }

        [HttpGet("customer")]
        public async Task<IActionResult> GetRatingsByCustomer(int? pageIndex, int? pageSize)
        {
            var result = await _ratingService.GetRatingsByCustomer(User, pageIndex, pageSize);

            //if (result.TotalCount == 0)
            //    return NotFound(new AppResponse<PagingRatingResponseListDTO>().SetErrorResponse(KeyConst.Rating, RatingConst.NotFoundError));

            return Ok(new AppResponse<PagingRatingResponseListDTO>().SetSuccessResponse(result));
        }

        [HttpGet("service/{serviceId}")]
        public async Task<IActionResult> GetRatingsByService(Guid serviceId, int? pageIndex, int? pageSize)
        {
            var result = await _ratingService.GetRatingsByService(serviceId, pageIndex, pageSize);

            //if (result.TotalCount == 0)
            //    return NotFound(new AppResponse<PagingRatingResponseListDTO>().SetErrorResponse(KeyConst.Rating, RatingConst.NotFoundError));

            return Ok(new AppResponse<PagingRatingResponseListDTO>().SetSuccessResponse(result));
        }

        [HttpGet("service/{serviceId}/filter")]
        public async Task<IActionResult> FilterRatings(Guid serviceId, decimal rate, int? pageIndex, int? pageSize)
        {
            var result = await _ratingService.SortRatings(serviceId, rate,pageIndex, pageSize);

            //if (result.TotalCount == 0)
            //    return NotFound(new AppResponse<PagingRatingResponseListDTO>().SetErrorResponse(KeyConst.Rating, RatingConst.NotFoundError));

            return Ok(new AppResponse<PagingRatingResponseListDTO>().SetSuccessResponse(result));
        }

        [HttpGet("housekeeper/{housekeeperId}")]
        public async Task<IActionResult> GetHousekeeperRating(string housekeeperId)
        {
            var result = await _ratingService.GetHousekeeperRatingAsync(housekeeperId);
           
            if(User == null)
            {
                return NotFound(new AppResponse<AppUser>().SetErrorResponse(KeyConst.Unathorized, CommonConst.UnauthorizeError));
            }

            if (result.HousekeeperId.IsNullOrEmpty())
            {
                return NotFound(new AppResponse<HousekeperRatingDTO>().SetErrorResponse(KeyConst.Rating, RatingConst.NotFoundError));
            }
            return Ok(new AppResponse<HousekeperRatingDTO>().SetSuccessResponse(result));
        }
    }
}
