using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class GoongDistanceService : IGoongDistanceService
{
    private readonly HttpClient _httpClient;
    private const string GoongApiKey = "N9JxG9aqmBVPRdSlAJxvCgFp7VeDMcuH3a9YmD5H";
    private const string GoongApiUrl = "https://rsapi.goong.io/DistanceMatrix";
    private readonly ILogger<GoongDistanceService> _logger; // Inject ILogger in the constructor


    public GoongDistanceService(HttpClient httpClient, ILogger<GoongDistanceService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }


    public async Task<double?> GetDistanceAsync(string originPlaceId, string destinationPlaceId)
    {
        try
        {
            var originCoordinates = await GetLatLngFromPlaceId(originPlaceId);
            var destinationCoordinates = await GetLatLngFromPlaceId(destinationPlaceId);

            if (originCoordinates == null || destinationCoordinates == null)
            {
                _logger.LogError("Failed to retrieve coordinates. Origin: {Origin}, Destination: {Destination}", originPlaceId, destinationPlaceId);
                return null;
            }

            var url = $"{GoongApiUrl}?origins={originCoordinates}&destinations={destinationCoordinates}&vehicle=car&api_key={GoongApiKey}";
            _logger.LogInformation("Calling Goong Distance API: {Url}", url);

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Goong Distance API request failed. Status Code: {StatusCode}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.GetProperty("rows").GetArrayLength() > 0 &&
                root.GetProperty("rows")[0].GetProperty("elements").GetArrayLength() > 0)
            {
                var distance = root.GetProperty("rows")[0].GetProperty("elements")[0]
                                .GetProperty("distance").GetProperty("value").GetDouble();
                _logger.LogInformation("Distance retrieved: {Distance} meters", distance);

                return distance / 1000.0; // Convert to kilometers
            }

            _logger.LogWarning("No distance data found in API response.");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving the distance.");
            return null;
        }
    }

    private async Task<string?> GetLatLngFromPlaceId(string placeId)
    {
        try
        {
            var placeDetailsUrl = $"https://rsapi.goong.io/Place/Detail?place_id={placeId}&api_key={GoongApiKey}";
            _logger.LogInformation("Calling Goong Place Details API: {Url}", placeDetailsUrl);

            var response = await _httpClient.GetAsync(placeDetailsUrl);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Goong Place Details API request failed. Status Code: {StatusCode}", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("result", out var result) && result.TryGetProperty("geometry", out var geometry))
            {
                var lat = geometry.GetProperty("location").GetProperty("lat").GetDouble();
                var lng = geometry.GetProperty("location").GetProperty("lng").GetDouble();
                var coordinates = $"{lat},{lng}";

                _logger.LogInformation("Coordinates retrieved for Place ID {PlaceId}: {Coordinates}", placeId, coordinates);
                return coordinates;
            }

            _logger.LogWarning("No location data found for Place ID: {PlaceId}", placeId);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving coordinates for Place ID: {PlaceId}", placeId);
            return null;
        }
    }


}
