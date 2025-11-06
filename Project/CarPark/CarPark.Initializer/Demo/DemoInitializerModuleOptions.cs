using System.ComponentModel.DataAnnotations;

namespace CarPark.Initializer.Demo;

internal class DemoInitializerModuleOptions
{
    [Required]
    public required string ConnectionString { get; set; }

    [Required]
    public required string GraphHopperApiKey { get; set; }
}