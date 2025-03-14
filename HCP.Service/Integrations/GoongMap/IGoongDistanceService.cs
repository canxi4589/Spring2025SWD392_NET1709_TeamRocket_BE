
using HCP.Repository.Entities;

public interface IGoongDistanceService
{
    Task<List<CleaningService>> GetServicesWithinDistanceAsync(string userPlaceId, double maxDistanceKm);
    Task<double?> GetDistanceAsync(string originPlaceId, string destinationPlaceId);
    Task<List<CleaningService>> GetServicesWithinDistanceAsync(string userPlaceId, double maxDistanceKm, List<CleaningService> services);
    Task<List<CleaningService>> GetBookableServicesWithinDistanceAsync(
    string userPlaceId, List<CleaningService> services);
}