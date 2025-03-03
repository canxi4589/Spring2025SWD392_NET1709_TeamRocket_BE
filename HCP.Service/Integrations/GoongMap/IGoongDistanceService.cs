
public interface IGoongDistanceService
{
    Task<double?> GetDistanceAsync(string originPlaceId, string destinationPlaceId);
}