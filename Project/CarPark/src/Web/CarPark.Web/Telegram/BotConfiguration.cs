using System.ComponentModel.DataAnnotations;

namespace CarPark.Telegram;

internal sealed class BotConfiguration
{
    public const string Key = "BotConfiguration";

    [Required]
    public required string BotToken { get; init; }
}