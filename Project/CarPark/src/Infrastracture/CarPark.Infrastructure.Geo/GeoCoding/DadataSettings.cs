using System.ComponentModel.DataAnnotations;

namespace CarPark.Geo.GeoCoding;

public class DadataSettings
{
    public const string ConfigurationSectionName = "Dadata";

    [Required]
    public required string Token { get; set; }
    
    public string? Secret { get; set; }

    public string? BaseUrl { get; set; } = "https://suggestions.dadata.ru/suggestions/api/4_1/rs";
}