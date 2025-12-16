using System.Collections.Concurrent;
using CarPark.Data;
using CarPark.Managers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Telegram.Services;

internal interface ITelegramAuthenticationService
{
    Task<bool> AuthenticateAsync(long telegramUserId, string username, string password);
    bool IsAuthenticated(long telegramUserId);
    long? FindManagersTelegramId(Guid managerId);
    Task<Manager?> GetAuthenticatedManagerAsync(long telegramUserId);
}

internal sealed class TelegramAuthenticationService : ITelegramAuthenticationService
{
    private readonly ConcurrentDictionary<long, Manager> _authenticatedUsers = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TelegramAuthenticationService> _logger;

    public TelegramAuthenticationService(IServiceProvider serviceProvider, ILogger<TelegramAuthenticationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<bool> AuthenticateAsync(long telegramUserId, string username, string password)
    {
        try
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            
            UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Find the IdentityUser by username
            IdentityUser? identityUser = await userManager.FindByNameAsync(username);
            
            if (identityUser == null)
            {
                _logger.LogWarning("User not found: {Username}", username);
                return false;
            }

            // Verify password using UserManager
            bool passwordValid = await userManager.CheckPasswordAsync(identityUser, password);
            
            if (!passwordValid)
            {
                _logger.LogWarning("Invalid password for user: {Username}", username);
                return false;
            }

            // Find the corresponding Manager
            Manager? manager = await dbContext.Managers
                .Include(m => m.Enterprises)
                .FirstOrDefaultAsync(m => m.IdentityUserId == identityUser.Id);

            if (manager != null)
            {
                _authenticatedUsers.AddOrUpdate(telegramUserId, manager, (key, oldValue) => manager);
                _logger.LogInformation("User {TelegramUserId} authenticated as manager {ManagerId}", telegramUserId, manager.Id);
                return true;
            }

            _logger.LogWarning("Manager not found for IdentityUser: {IdentityUserId}", identityUser.Id);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication for user {TelegramUserId}", telegramUserId);
            return false;
        }
    }

    public bool IsAuthenticated(long telegramUserId)
    {
        return _authenticatedUsers.ContainsKey(telegramUserId);
    }

    public async Task<Manager?> GetAuthenticatedManagerAsync(long telegramUserId)
    {
        if (_authenticatedUsers.TryGetValue(telegramUserId, out Manager manager))
        {
            // Refresh manager data from database to ensure enterprises are up to date
            using IServiceScope scope = _serviceProvider.CreateScope();
            ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            Manager? freshManager = await dbContext.Managers
                .Include(m => m.Enterprises)
                .FirstOrDefaultAsync(m => m.Id == manager.Id);
                
            if (freshManager != null)
            {
                _authenticatedUsers.AddOrUpdate(telegramUserId, freshManager, (key, oldValue) => freshManager);
                return freshManager;
            }
        }
        
        return null;
    }

    public long? FindManagersTelegramId(Guid managerId)
    {
        KeyValuePair<long, Manager>? manager = _authenticatedUsers
            .FirstOrDefault(u => u.Value.Id == managerId);

        return manager?.Key;
    }
}