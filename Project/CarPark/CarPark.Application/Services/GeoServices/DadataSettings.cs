namespace CarPark.Services.GeoServices;

public class DadataSettings
{
    public const string Key = "Dadata";

    public required string Token { get; set; }

    public string? Secret { get; set; }

    public string? BaseUrl { get; set; } = "https://suggestions.dadata.ru/suggestions/api/4_1/rs";
}