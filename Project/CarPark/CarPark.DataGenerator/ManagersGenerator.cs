using Bogus;
using CarPark.Managers;
using CarPark.Managers.Services;
using CarPark.Enterprises;
using Microsoft.AspNetCore.Identity;
using FluentResults;

namespace CarPark.DataGenerator;

public class ManagersGenerator
{
    private readonly int _seed;
    private readonly Random _random;
    private readonly IManagersService _managersService;

    public ManagersGenerator(IManagersService managersService, int seed)
    {
        _seed = seed;
        _random = new Random(seed);
        _managersService = managersService;
    }

    public (List<IdentityUser> identityUsers, List<Manager> managers) GenerateManagers(
        List<Enterprise> enterprises)
    {
        Faker faker = new Faker("ru") { Random = new Randomizer(_seed) };
        PasswordHasher<IdentityUser> hasher = new PasswordHasher<IdentityUser>();

        List<IdentityUser> identityUsers = new List<IdentityUser>();
        List<Manager> managers = new List<Manager>();

        int managerCounter = 1;

        // Генерируем 1-2 менеджера на предприятие
        foreach (Enterprise enterprise in enterprises)
        {
            int managersCount = faker.Random.Int(1, 2);

            for (int i = 0; i < managersCount; i++)
            {
                string username = $"manager{managerCounter}";

                IdentityUser identityUser = CreateIdentityUser(username, hasher);
                identityUsers.Add(identityUser);

                Manager manager = CreateManager(identityUser.Id, enterprise);
                managers.Add(manager);

                managerCounter++;
            }
        }

        return (identityUsers, managers);
    }

    private IdentityUser CreateIdentityUser(string username, PasswordHasher<IdentityUser> hasher)
    {
        string userId = GenerateDeterministicGuid().ToString();
        string securityStamp = GenerateDeterministicGuid().ToString();
        string concurrencyStamp = GenerateDeterministicGuid().ToString();

        IdentityUser identityUser = new IdentityUser
        {
            Id = userId,
            UserName = username,
            NormalizedUserName = username.ToUpperInvariant(),
            SecurityStamp = securityStamp,
            ConcurrencyStamp = concurrencyStamp
        };
        identityUser.PasswordHash = hasher.HashPassword(identityUser, "123456");

        return identityUser;
    }

    private Manager CreateManager(string identityUserId, Enterprise enterprise)
    {
        Guid managerId = GenerateDeterministicGuid();

        CreateManagerRequest request = new CreateManagerRequest
        {
            Id = managerId,
            IdentityUserId = identityUserId,
            Enterprises = new List<Enterprise> { enterprise }
        };

        Result<Manager> result = _managersService.CreateManager(request);
        
        if (result.IsFailed)
        {
            throw new InvalidOperationException($"Failed to create manager: {string.Join(", ", result.Errors.Select(e => e.Message))}");
        }

        return result.Value;
    }

    private Guid GenerateDeterministicGuid()
    {
        byte[] bytes = new byte[16];
        _random.NextBytes(bytes);
        return new Guid(bytes);
    }
}