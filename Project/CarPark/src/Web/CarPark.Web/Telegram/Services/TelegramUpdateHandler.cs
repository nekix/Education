using CarPark.DateTimes;
using CarPark.Managers;
using CarPark.Reports.Abstract;
using FluentResults;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace CarPark.Telegram.Services;

internal sealed record DateRangeParseResult(
    UtcDateTimeOffset StartDate,
    UtcDateTimeOffset EndDate,
    PeriodType Period);

internal sealed class TelegramUpdateHandler : IUpdateHandler
{
    private readonly ITelegramBotClient _bot;
    private readonly ILogger<TelegramUpdateHandler> _logger;
    private readonly ITelegramAuthenticationService _authenticationService;
    private readonly ITelegramReportService _reportService;

    public TelegramUpdateHandler(
        ITelegramBotClient bot, 
        ILogger<TelegramUpdateHandler> logger,
        ITelegramAuthenticationService authenticationService,
        ITelegramReportService reportService)
    {
        _bot = bot;
        _logger = logger;
        _authenticationService = authenticationService;
        _reportService = reportService;
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source, CancellationToken cancellationToken)
    {
        _logger.LogInformation("HandleError: {Exception}", exception);
        // Cooldown in case of network connection error
        if (exception is RequestException)
            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Только приватные чаты
        if (update.Message?.Chat.Type != ChatType.Private)
            return;

        await (update switch
        {
            { Message: { } message } => OnMessage(message),
            _ => UnknownUpdateHandlerAsync(update)
        });
    }

    private async Task OnMessage(Message msg)
    {
        _logger.LogInformation("Receive message type: {MessageType}", msg.Type);
        if (msg.Text is not { } messageText)
            return;

        Message sentMessage = await (messageText.Split(' ')[0] switch
        {
            "/login" => SendLogin(msg),
            "/summary_vehicle" => SendVehicleSummary(msg, messageText),
            "/summary_enterprise" => SendEnterpriseSummary(msg, messageText),
            "/help" => SendHelp(msg),
            _ => Usage(msg)
        });
        _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.Id);
    }

    async Task<Message> Usage(Message msg)
    {
        // Check if user is authenticated
        if (!_authenticationService.IsAuthenticated(msg.From.Id))
        {
            return await _bot.SendMessage(msg.Chat, "Доступные команды:\n<b>/login</b> - вход в систему\n<b>/help</b> - справка", parseMode: ParseMode.Html);
        }
        
        return await _bot.SendMessage(msg.Chat, 
            "Доступные команды:\n<b>/login</b> - вход в систему\n<b>/summary_vehicle</b> - сводка по автомобилю\n<b>/summary_enterprise</b> - сводка по предприятию\n<b>/help</b> - справка", 
            parseMode: ParseMode.Html);
    }

    async Task<Message> SendHelp(Message msg)
    {
        StringBuilder sb = new();
        sb.AppendLine("🤖 <b>Справка по боту</b>");
        sb.AppendLine();
        sb.AppendLine("📋 <b>Команды:</b>");
        sb.AppendLine("<b>/login username password</b> - авторизация");
        sb.AppendLine("<b>/summary_vehicle vehicle_id date</b> - отчет по автомобилю");
        sb.AppendLine("<b>/summary_enterprise enterprise_id date</b> - отчет по предприятию");
        sb.AppendLine("<b>/help</b> - эта справка");
        sb.AppendLine();
        sb.AppendLine("📅 <b>Форматы дат:</b>");
        sb.AppendLine("• ДД.ММ.ГГГГ - день (например, 25.12.2025)");
        sb.AppendLine("• ММ.ГГГГ - месяц (например, 12.2025)");
        sb.AppendLine("• ДД.ММ.ГГ - день с коротким годом (например, 25.12.25)");
        sb.AppendLine("• ММ.ГГ - месяц с коротким годом (например, 12.25)");
        sb.AppendLine();
        sb.AppendLine("📝 <b>Примеры:</b>");
        sb.AppendLine("<code>/summary_vehicle 123e4567-e89b-12d3-a456-426614174000 25.12.2025</code>");
        sb.AppendLine("<code>/summary_enterprise 987fcdeb-51a2-43d1-9c4f-123456789abc 12.2025</code>");

        return await _bot.SendMessage(msg.Chat, sb.ToString(), parseMode: ParseMode.Html);
    }

    async Task<Message> SendLogin(Message msg)
    {
        // Parse command arguments: /login username password
        string[] parts = msg.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3)
        {
            return await _bot.SendMessage(msg.Chat, "❌ Неверный формат команды. Используйте: <code>/login username password</code>", parseMode: ParseMode.Html);
        }

        string username = parts[1];
        string password = parts[2];

        bool isAuthenticated = await _authenticationService.AuthenticateAsync(msg.From.Id, username, password);

        if (isAuthenticated)
        {
            Manager manager = await _authenticationService.GetAuthenticatedManagerAsync(msg.From.Id);
            if (manager is not null)
            {
                return await _bot.SendMessage(msg.Chat,
                    $"✅ Успешная авторизация!\n👤 Менеджер ID: `{manager.Id}`\n🏢 Предприятий: {manager.Enterprises.Count}\n\nДоступные команды: /summary_vehicle, /summary_enterprise",
                    parseMode: ParseMode.Html);
            }
            else
            {
                return await _bot.SendMessage(msg.Chat, "❌ Ошибка получения данных менеджера");
            }
        }
        else
        {
            return await _bot.SendMessage(msg.Chat, "❌ Неверный логин или пароль");
        }
    }

    async Task<Message> SendVehicleSummary(Message msg, string messageText)
    {
        // Check authentication
        if (!_authenticationService.IsAuthenticated(msg.From.Id))
        {
            return await _bot.SendMessage(msg.Chat, "❌ Необходима авторизация. Используйте <code>/login username password</code>", parseMode: ParseMode.Html);
        }

        // Parse command: /summary_vehicle vehicle_id date
        string[] parts = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3)
        {
            return await _bot.SendMessage(msg.Chat, "❌ Неверный формат команды. Используйте: <code>/summary_vehicle vehicle_id date</code>", parseMode: ParseMode.Html);
        }

        if (!Guid.TryParse(parts[1], out Guid vehicleId))
        {
            return await _bot.SendMessage(msg.Chat, "❌ Неверный формат ID автомобиля");
        }

        try
        {
            Manager manager = await _authenticationService.GetAuthenticatedManagerAsync(msg.From.Id);
            if (manager is null)
            {
                return await _bot.SendMessage(msg.Chat, "❌ Ошибка получения данных менеджера");
            }

            Result<DateRangeParseResult> dateResult = ParseDateRange(parts[2]);
            if (dateResult.IsFailed)
            {
                return await _bot.SendMessage(msg.Chat, $"❌ Ошибка в формате даты: {string.Join(", ", dateResult.Errors.Select(e => e.Message))}", parseMode: ParseMode.Html);
            }

            DateRangeParseResult dateRange = dateResult.Value;

            string report = await _reportService.GetVehicleMileageReportAsync(manager.Id, vehicleId, dateRange.StartDate, dateRange.EndDate, dateRange.Period);

            return await _bot.SendMessage(msg.Chat, report, parseMode: ParseMode.Html);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting vehicle summary for user {UserId}, vehicle {VehicleId}", msg.From.Id, vehicleId);
            return await _bot.SendMessage(msg.Chat, "❌ Произошла ошибка при получении отчета");
        }
    }

    async Task<Message> SendEnterpriseSummary(Message msg, string messageText)
    {
        // Check authentication
        if (!_authenticationService.IsAuthenticated(msg.From.Id))
        {
            return await _bot.SendMessage(msg.Chat, "❌ Необходима авторизация. Используйте <code>/login username password</code>", parseMode: ParseMode.Html);
        }

        // Parse command: /summary_enterprise enterprise_id date
        string[] parts = messageText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3)
        {
            return await _bot.SendMessage(msg.Chat, "❌ Неверный формат команды. Используйте: <code>/summary_enterprise enterprise_id date</code>", parseMode: ParseMode.Html);
        }

        if (!Guid.TryParse(parts[1], out Guid enterpriseId))
        {
            return await _bot.SendMessage(msg.Chat, "❌ Неверный формат ID предприятия");
        }

        try
        {
            Manager manager = await _authenticationService.GetAuthenticatedManagerAsync(msg.From.Id);
            if (manager is null)
            {
                return await _bot.SendMessage(msg.Chat, "❌ Ошибка получения данных менеджера");
            }

            Result<DateRangeParseResult> dateResult = ParseDateRange(parts[2]);
            if (dateResult.IsFailed)
            {
                return await _bot.SendMessage(msg.Chat, $"❌ Ошибка в формате даты: {string.Join(", ", dateResult.Errors.Select(e => e.Message))}", parseMode: ParseMode.Html);
            }

            DateRangeParseResult dateRange = dateResult.Value;

            string report = await _reportService.GetEnterpriseMileageReportAsync(manager.Id, enterpriseId, dateRange.StartDate, dateRange.EndDate, dateRange.Period);

            return await _bot.SendMessage(msg.Chat, report, parseMode: ParseMode.Html);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting enterprise summary for user {UserId}, enterprise {EnterpriseId}", msg.From.Id, enterpriseId);
            return await _bot.SendMessage(msg.Chat, "❌ Произошла ошибка при получении отчета");
        }
    }

    private static Result<DateRangeParseResult> ParseDateRange(string dateInput)
    {
        try
        {
            // Remove leading/trailing whitespace
            dateInput = dateInput.Trim();

            //M.YYYY format Try parsing DD.M (day)
            if (DateTimeOffset.TryParseExact(dateInput, "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out DateTimeOffset parsedDate))
            {
                UtcDateTimeOffset startDate = new UtcDateTimeOffset(parsedDate.Date);
                UtcDateTimeOffset endDate = new UtcDateTimeOffset(parsedDate.Date.AddDays(1).AddTicks(-1));
                return Result.Ok(new DateRangeParseResult(startDate, endDate, PeriodType.Day));
            }

            // Try parsing MM.YYYY format (month)
            if (DateTimeOffset.TryParseExact(dateInput, "MM.yyyy", null, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                DateTimeOffset startDto = new DateTimeOffset(parsedDate.Year, parsedDate.Month, 1, 0, 0, 0, TimeSpan.Zero);
                DateTimeOffset endDto = startDto.AddMonths(1).AddTicks(-1);
                UtcDateTimeOffset startDate = new UtcDateTimeOffset(startDto);
                UtcDateTimeOffset endDate = new UtcDateTimeOffset(endDto);
                return Result.Ok(new DateRangeParseResult(startDate, endDate, PeriodType.Month));
            }

            // Try parsing DD.MM.YY format (day with 2-digit year)
            if (DateTimeOffset.TryParseExact(dateInput, "dd.MM.yy", null, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                DateTimeOffset startDto = parsedDate.Date;
                DateTimeOffset endDto = startDto.AddDays(1).AddTicks(-1);
                UtcDateTimeOffset startDate = new UtcDateTimeOffset(startDto);
                UtcDateTimeOffset endDate = new UtcDateTimeOffset(endDto);
                return Result.Ok(new DateRangeParseResult(startDate, endDate, PeriodType.Day));
            }

            // Try parsing MM.YY format (month with 2-digit year)
            if (DateTimeOffset.TryParseExact(dateInput, "MM.yy", null, System.Globalization.DateTimeStyles.None, out parsedDate))
            {
                DateTimeOffset startDto = new DateTimeOffset(parsedDate.Year, parsedDate.Month, 1, 0, 0, 0, TimeSpan.Zero);
                DateTimeOffset endDto = startDto.AddMonths(1).AddTicks(-1);
                UtcDateTimeOffset startDate = new UtcDateTimeOffset(startDto);
                UtcDateTimeOffset endDate = new UtcDateTimeOffset(endDto);
                return Result.Ok(new DateRangeParseResult(startDate, endDate, PeriodType.Month));
            }

            Error error = new Error($"Не удалось распознать формат даты: {dateInput}. Используйте форматы: ДД.ММ.ГГГГ (день) или ММ.ГГГГ (месяц)");
            return Result.Fail<DateRangeParseResult>(error);
        }
        catch (Exception ex)
        {
            Error error = new Error($"Ошибка при парсинге даты: {ex.Message}");
            return Result.Fail<DateRangeParseResult>(error);
        }
    }

    private Task UnknownUpdateHandlerAsync(Update update)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}