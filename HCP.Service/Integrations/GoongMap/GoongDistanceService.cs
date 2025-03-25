using HCP.Repository.Entities;
using HCP.Repository.Enums;
using HCP.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class GoongDistanceService : IGoongDistanceService
{
    private readonly HttpClient _httpClient;
    private const string GoongApiKey = "hvloh0IRgo8MWKFNF0wtIGtRnKBYpbfCWNb7XLdm";
    private const string GoongApiUrl = "https://rsapi.goong.io/DistanceMatrix";
    private readonly ILogger<GoongDistanceService> _logger; // Inject ILogger in the constructor
    private readonly IUnitOfWork _unitOfWork;

    public GoongDistanceService(HttpClient httpClient, ILogger<GoongDistanceService> logger, IUnitOfWork unitOfWork)
    {
        _httpClient = httpClient;
        _logger = logger;
        _unitOfWork = unitOfWork;
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

                return distance / 1000.0; 
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

    public async Task<string?> GetLatLngFromPlaceId(string placeId)
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
                var coordinates = string.Format(CultureInfo.InvariantCulture, "{0},{1}", lat, lng);

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
    public async Task<List<CleaningService>> GetServicesWithinDistanceAsync(string userPlaceId, double maxDistanceKm)
    {
        try
        {
            var userCoordinates = await GetLatLngFromPlaceId(userPlaceId);
            if (userCoordinates == null)
            {
                _logger.LogError("Failed to retrieve coordinates for user: {UserPlaceId}", userPlaceId);
                return new List<CleaningService>();
            }

            // Get all cleaning services from the database
            var services = await _unitOfWork.Repository<CleaningService>().GetAll().ToListAsync();
            var servicePlaceIds = services.Select(s => s.PlaceId).ToList();

            var distances = await GetDistancesAsyncByList(userPlaceId, servicePlaceIds);

            if (distances == null)
            {
                _logger.LogError("Failed to retrieve distances.");
                return new List<CleaningService>();
            }

            // Filter services based on maxDistanceKm
            var filteredServices = services
                .Where(service => distances.ContainsKey(service.PlaceId) && distances[service.PlaceId] <= maxDistanceKm)
                .ToList();

            return filteredServices;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while filtering services.");
            return new List<CleaningService>();
        }
    }
    public async Task<List<CleaningService>> GetServicesWithinDistanceAsync(string userPlaceId, double maxDistanceKm, List<CleaningService> services)
    {
        try
        {
            var servicePlaceIds = services.Select(s => s.PlaceId).ToList();
            var distances = await GetDistancesAsyncByList(userPlaceId, servicePlaceIds);

            if (distances == null)
            {
                _logger.LogError("Failed to retrieve distances.");
                return new List<CleaningService>();
            }

            // Filter only the provided services
            return services.Where(service => distances.ContainsKey(service.PlaceId) && distances[service.PlaceId] <= maxDistanceKm).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while filtering services.");
            return new List<CleaningService>();
        }
    }
    public async Task<List<CleaningService>> GetBookableServicesWithinDistanceAsync(
    string userPlaceId, List<CleaningService> services)
    {
        try
        {
            var servicePlaceIds = services.Select(s => s.PlaceId).ToList();

            // Retrieve distances
            var distances = await GetDistancesAsyncByList(userPlaceId, servicePlaceIds);

            if (distances == null)
            {
                _logger.LogError("Failed to retrieve distances.");
                return new List<CleaningService>();
            }
              //.Where(service =>
              //      distances.ContainsKey(service.PlaceId) &&
              //      IsServiceBookable(service, distances[service.PlaceId]))
            return services
              
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while filtering bookable services.");
            return new List<CleaningService>();
        }
    }

    private bool IsServiceBookable(CleaningService service, double distance)
    {
        return service.Status == ServiceStatus.Active.ToString() &&
               service.ServiceTimeSlots.Any(slot => slot.Status != "hehe") && 
               service.DistancePricingRules.Any(rule =>
                   rule.IsActive &&
                   distance >= rule.MinDistance &&
                   distance <= rule.MaxDistance); 
    }
    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth radius in km
        double dLat = (lat2 - lat1) * Math.PI / 180;
        double dLon = (lon2 - lon1) * Math.PI / 180;

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c; // Distance in km
    }

    private async Task<Dictionary<string, double>> GetDistancesAsyncByList(string originPlaceId, List<string> destinationPlaceIds)
    {
        var distances = new Dictionary<string, double>();

        if (!destinationPlaceIds.Any())
        {
            _logger.LogWarning("No destination Place IDs provided.");
            return distances;
        }

        try
        {
            var origin = await GetLatLngFromPlaceId(originPlaceId);
            if (string.IsNullOrEmpty(origin))
            {
                _logger.LogWarning("Invalid origin coordinates for Place ID: {OriginPlaceId}", originPlaceId);
                return distances;
            }

            var destinationTasks = destinationPlaceIds.Select(async id => (id, await GetLatLngFromPlaceId(id))).ToList();
            var destinationResults = await Task.WhenAll(destinationTasks);

            var validDestinations = destinationResults
                .Where(r => !string.IsNullOrEmpty(r.Item2))
                .Select(r => r.Item2)
                .ToList();
            if (!validDestinations.Any())
            {
                _logger.LogWarning("No valid destination coordinates retrieved.");
                return distances;
            }

            var destinations = string.Join("|", validDestinations);
            var url = $"https://rsapi.goong.io/DistanceMatrix?origins={origin}&destinations={destinations}&vehicle=car&api_key={GoongApiKey}";
            _logger.LogInformation("Calling Goong Distance API: {Url}", url);

            // Make the API call
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Goong Distance API request failed. Status Code: {StatusCode}", response.StatusCode);
                return distances;
            }

            // Parse the response
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var rows = root.GetProperty("rows");
            if (rows.GetArrayLength() == 0)
            {
                _logger.LogWarning("No distance data found in API response.");
                return distances;
            }

            var elements = rows[0].GetProperty("elements");
            var validDestinationPlaceIds = destinationResults
                .Where(r => !string.IsNullOrEmpty(r.Item2))
                .Select(r => r.id)
                .ToList();

            for (int i = 0; i < elements.GetArrayLength() && i < validDestinationPlaceIds.Count; i++)
            {
                if (elements[i].TryGetProperty("distance", out var distanceElement))
                {
                    var distanceInMeters = distanceElement.GetProperty("value").GetDouble();
                    distances[validDestinationPlaceIds[i]] = distanceInMeters / 1000.0;
                }
                else
                {
                    _logger.LogWarning("No distance found for destination: {Destination}", validDestinationPlaceIds[i]);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving distances.");
        }

        return distances;
    }
}
