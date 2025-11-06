using System.ComponentModel.DataAnnotations;

namespace CarPark.Initializer.Minimal;

public sealed class MinimalInitializerModuleOptions
{
    public const string Key = "App";

    [Required]
    public required string ConnectionString { get; set; }
}