using HCP.Repository.Constance;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/distance")]
public class MapController : ControllerBase
{
    private readonly IGoongDistanceService _yourService;
    private readonly ILogger<MapController> _logger;

    public MapController(IGoongDistanceService yourService, ILogger<MapController> logger)
    {
        _yourService = yourService;
        _logger = logger;
    }

    [HttpGet("calculate")]
    public async Task<IActionResult> GetDistance([FromQuery] string originPlaceId, [FromQuery] string destinationPlaceId)
    {
        try
        {
            _logger.LogInformation("Received request to get distance between {Origin} and {Destination}", originPlaceId, destinationPlaceId);

            var distance = await _yourService.GetDistanceAsync(originPlaceId, destinationPlaceId);

            if (distance == null)
            {
                _logger.LogWarning("Distance could not be calculated for Origin: {Origin}, Destination: {Destination}", originPlaceId, destinationPlaceId);
                return NotFound(new { message = CommonConst.NotFoundError });
            }

            return Ok(new { distanceInKm = distance });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, CommonConst.SomethingWrongMessage);
            return StatusCode(500, new { message = CommonConst.InternalError });
        }
    }
}
