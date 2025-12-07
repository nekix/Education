using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace CarPark.Telegram.Services;

internal sealed class TelegramBotService : BackgroundService
{
    private readonly ITelegramBotClient _bot;
    private readonly IUpdateHandler _updateHandler;
    private readonly ILogger<TelegramBotService> _logger;

    public TelegramBotService(
        ITelegramBotClient bot,
        IUpdateHandler updateHandler,
        ILogger<TelegramBotService> logger)
    {
        _bot = bot;
        _updateHandler = updateHandler;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Telegram Bot Service starting...");

        try
        {
            // Start receiving updates
            await _bot.ReceiveAsync(
                updateHandler: _updateHandler,
                cancellationToken: stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Telegram Bot Service is stopping...");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Telegram Bot Service");
            throw;
        }
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting Telegram Bot Service...");

        // Test bot connection
        User me = await _bot.GetMe(cancellationToken);
        _logger.LogInformation($"Telegram Bot '{me.Username}' (ID: {me.Id}) started successfully");

        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Telegram Bot Service...");
        await base.StopAsync(cancellationToken);
    }
}