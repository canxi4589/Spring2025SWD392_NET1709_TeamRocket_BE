using Azure;
using HCP.Repository.Constance;
using HCP.Service.Services.CustomerService;
using HCP.Service.Services.HousekeeperService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HousekeeperController : ControllerBase
    {
        private readonly IHousekeeperService _housekeeperService;

        public HousekeeperController(IHousekeeperService housekeeperService)
        {
            _housekeeperService = housekeeperService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetHousekeeperProfile()
        {
            var response = new AppResponse<string>();
            try
            {
                var housekeeper = await _housekeeperService.GetHousekeeperProfile(User);
                var successResponse = new AppResponse<object>()
                .SetSuccessResponse(housekeeper);

                return Ok(successResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(response.SetErrorResponse(KeyConst.Housekeeper, ex.ToString()));
            }
        }
    }
}
