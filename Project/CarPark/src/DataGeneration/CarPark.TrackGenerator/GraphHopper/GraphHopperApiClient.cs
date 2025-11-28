using System.Text;
using System.Text.Json;
using CarPark.TrackGenerator.GraphHopper.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CarPark.TrackGenerator.GraphHopper;

public class GraphHopperApiClient : IGraphHopperApiClient
{
    private readonly HttpClient _httpClient;
    private readonly GraphHopperOptions _options;
    private readonly ILogger<GraphHopperApiClient> _logger;

    public GraphHopperApiClient(
        HttpClient httpClient, 
        IOptions<GraphHopperOptions> options,
        ILogger<GraphHopperApiClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<RouteResponse> GetRouteAsync(RouteRequest request)
    {
        string url = $"https://graphhopper.com/api/1/route?key={_options.ApiKey}";
        
        string json = JsonSerializer.Serialize(request);

        _logger.LogDebug("Sending GraphHopper request to {Url} with payload: {Json}", url, json);

        StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        
        try
        {
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("GraphHopper API error {StatusCode}: {Error}", 
                    response.StatusCode, errorContent);
                throw new GraphHopperApiException($"API request failed with status {response.StatusCode}: {errorContent}");
            }

            string responseJson = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("GraphHopper response: {Response}", responseJson);

            RouteResponse? routeResponse = JsonSerializer.Deserialize<RouteResponse>(responseJson);

            return routeResponse ?? throw new GraphHopperApiException("Failed to deserialize response");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling GraphHopper API");
            throw new GraphHopperApiException("Network error calling GraphHopper API", ex);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout calling GraphHopper API");
            throw new GraphHopperApiException("Timeout calling GraphHopper API", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse GraphHopper API response");
            throw new GraphHopperApiException("Failed to parse API response", ex);
        }
    }
}

public class GraphHopperOptions
{
    public string ApiKey { get; set; } = "";
    public int TimeoutSeconds { get; init; } = 30;
}

public class GraphHopperApiException : Exception
{
    public GraphHopperApiException(string message) : base(message) { }
    public GraphHopperApiException(string message, Exception innerException) : base(message, innerException) { }
}

