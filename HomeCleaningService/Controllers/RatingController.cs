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

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingController : ControllerBase
    {
        private readonly IServiceRatingService _ratingService;

        public RatingController(IServiceRatingService ratingService)
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

        [HttpGet("customer/{userId}")]
        public async Task<IActionResult> GetRatingsByCustomer(string userId)
        {
            var result = await _ratingService.GetRatingsByCustomer(userId);

            if (result.RatingCount == 0)
                return NotFound(new AppResponse<RatingResponseDTO>().SetErrorResponse(KeyConst.Rating, RatingConst.NotFoundError));

            return Ok(new AppResponse<RatingResponseDTO>().SetSuccessResponse(result));
        }

        [HttpGet("service/{serviceId}")]
        public async Task<IActionResult> GetRatingsByService(Guid serviceId)
        {
            var result = await _ratingService.GetRatingsByService(serviceId);

            if (result.RatingCount == 0)
                return NotFound(new AppResponse<RatingResponseDTO>().SetErrorResponse(KeyConst.Rating, RatingConst.NotFoundError));

            return Ok(new AppResponse<RatingResponseDTO>().SetSuccessResponse(result));
        }

        [HttpGet("service/{serviceId}/sort")]
        public async Task<IActionResult> SortRatings(Guid serviceId, [FromQuery] decimal minRating, [FromQuery] decimal maxRating)
        {
            var result = await _ratingService.SortRatings(serviceId, minRating, maxRating);

            if (result.RatingCount == 0)
                return NotFound(new AppResponse<RatingResponseDTO>().SetErrorResponse(KeyConst.Rating, RatingConst.NotFoundError));

            return Ok(new AppResponse<RatingResponseDTO>().SetSuccessResponse(result));
        }
    }
}
