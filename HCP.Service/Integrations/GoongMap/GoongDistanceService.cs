using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class GoongDistanceService : IGoongDistanceService
{
    private readonly HttpClient _httpClient;
    private const string GoongApiKey = "N9JxG9aqmBVPRdSlAJxvCgFp7VeDMcuH3a9YmD5H";
    private const string GoongApiUrl = "https://rsapi.goong.io/DistanceMatrix";

    public GoongDistanceService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<double?> GetDistanceAsync(string originPlaceId, string destinationPlaceId)
    {
        var url = $"{GoongApiUrl}?origins=place_id:{originPlaceId}&destinations=place_id:{destinationPlaceId}&api_key={GoongApiKey}";

        var response = await _httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (root.GetProperty("rows").GetArrayLength() > 0 &&
            root.GetProperty("rows")[0].GetProperty("elements").GetArrayLength() > 0)
        {
            var distance = root.GetProperty("rows")[0].GetProperty("elements")[0]
                            .GetProperty("distance").GetProperty("value").GetDouble();

            return distance / 1000.0; 
        }

        return null;
    }
}
