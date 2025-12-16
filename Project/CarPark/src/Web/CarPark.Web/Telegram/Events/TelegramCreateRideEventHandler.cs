using CarPark.Data;
using CarPark.Enterprises;
using CarPark.Rides.Events;
using CarPark.Telegram.Services;
using CarPark.Vehicles;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace CarPark.Telegram.Events;

internal class TelegramCreateRideEventHandler : ICreateRideEventHandler
{
    private readonly ApplicationDbContext _context;
    private readonly ITelegramAuthenticationService _telegramAuthService;
    private readonly ITelegramBotClient _bot;

    public TelegramCreateRideEventHandler(ApplicationDbContext context,
        ITelegramAuthenticationService telegramAuthService,
        ITelegramBotClient bot)
    {
        _context = context;
        _telegramAuthService = telegramAuthService;
        _bot = bot;
    }

    public async Task Handle(CreateRideEvent @event)
    {
        Enterprise enterprise = await _context.Enterprises
            .Include(e => e.Managers)
            .AsNoTracking()
            .FirstAsync(e => e.Id == @event.EnterpriseId);

        List<Guid> managersIds = enterprise.Managers
            .Select(m => m.Id)
            .ToList();

        Vehicle vehicle = await _context.Vehicles
            .AsNoTracking()
            .Include(v => v.Model)
            .FirstAsync(v => v.Id == @event.VehicleId);

        foreach (var chatId in managersIds.Select(_telegramAuthService.FindManagersTelegramId).Where(chatId => chatId != null))
        {
            await _bot.SendMessage(
                chatId!,
                $"""
                 <b>Создана новая поездка</b>

                 <b>Предприятие:</b> <code>{enterprise.Name}</code>
                 <b>Транспортное средство:</b> <code>{vehicle.VinNumber} - {vehicle.Model.ModelName}</code>
                 <b>Период:</b> {@event.StartTime.Value} — {@event.EndTime.Value}
                 <b>ID поездки:</b> <code>{@event.Rideid}</code>
                 """,
                ParseMode.Html
            );
        }
    }
}