using HCP.Service.DTOs;
using HCP.Service.Services.HomeCleaningService;
using HomeCleaningService.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeCleaningService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeServiceController : ControllerBase
    {
        private readonly ICleaningService cleaningService;

        public HomeServiceController(ICleaningService cleaningService)
        {
            this.cleaningService = cleaningService;
        }
        [HttpGet]
        public async Task<IActionResult> getAllService()
        {
            var list =await cleaningService.GetAllServiceWithoutCategoryAsync();
            var response = new AppResponse<List<HomeServiceDto>>().SetSuccessResponse(list);
            return Ok(response);
        }
        [HttpGet("{serviceId}/prices")]
        public async Task<IActionResult> GetServicePrices(Guid serviceId)
        {
            var prices = await cleaningService.GetServicePricesForCustomer(serviceId);
            var response = new AppResponse<List<ServicePricingDto>>().SetSuccessResponse(prices);
            return Ok(response);
        }
    }
}
