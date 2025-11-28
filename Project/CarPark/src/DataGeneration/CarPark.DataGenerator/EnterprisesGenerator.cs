using Bogus;
using CarPark.Enterprises;
using CarPark.Managers;
using CarPark.Drivers;
using CarPark.Vehicles;
using CarPark.TimeZones;
using CarPark.Enterprises.Services;
using FluentResults;

namespace CarPark.DataGenerator;

public class EnterprisesGenerator
{
    private readonly int _seed;
    private readonly Random _random;
    private readonly Faker _faker;
    private readonly IEnterprisesService _enterprisesService;

    public EnterprisesGenerator(IEnterprisesService enterprisesService, int seed = 42)
    {
        _seed = seed;
        _random = new Random(seed);
        _faker = new Faker("ru") { Random = new Randomizer(seed) };
        _enterprisesService = enterprisesService;
    }

    public List<Enterprise> GenerateEnterprises(int count)
    {
        Faker<EnterpriseCreateDataDto> enterpriseDataFaker = new Faker<EnterpriseCreateDataDto>("ru")
            .UseSeed(_seed)
            .RuleFor(d => d.Id, f => GenerateDeterministicGuid())
            .RuleFor(d => d.Name, f => f.Company.CompanyName())
            .RuleFor(d => d.LegalAddress, f => f.Address.FullAddress())
            .RuleFor(d => d.TimeZone, f => null);

        List<EnterpriseCreateDataDto> enterpriseData = enterpriseDataFaker.Generate(count);

        List<Enterprise> enterprises = new List<Enterprise>();
        foreach (var data in enterpriseData)
        {
            var createRequest = new CreateEnterpriseRequest
            {
                Id = data.Id,
                Name = data.Name,
                LegalAddress = data.LegalAddress,
                TimeZone = data.TimeZone
            };

            var result = _enterprisesService.CreateEnterprise(createRequest);
            if (result.IsSuccess)
            {
                enterprises.Add(result.Value);
            }
            else
            {
                throw new InvalidOperationException($"Failed to create enterprise: {string.Join(", ", result.Errors.Select(e => e.Message))}");
            }
        }

        return enterprises;
    }

    private class EnterpriseCreateDataDto
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public required string LegalAddress { get; set; }
        public required TzInfo? TimeZone { get; set; }
    }

    private Guid GenerateDeterministicGuid()
    {
        byte[] bytes = new byte[16];
        _random.NextBytes(bytes);
        return new Guid(bytes);
    }
}