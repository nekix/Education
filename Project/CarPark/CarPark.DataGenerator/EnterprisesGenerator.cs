using Bogus;
using CarPark.Enterprises;
using CarPark.Managers;
using CarPark.Drivers;
using CarPark.Vehicles;

namespace CarPark.DataGenerator;

public class EnterprisesGenerator
{
    private readonly int _seed;
    private readonly Faker _faker;

    public EnterprisesGenerator(int seed = 42)
    {
        _seed = seed;
        _faker = new Faker("ru") { Random = new Randomizer(seed) };
    }

    public List<Enterprise> GenerateEnterprises(int count)
    {
        Faker<Enterprise> enterpriseFaker = new Faker<Enterprise>("ru")
            .UseSeed(_seed)
            .RuleFor(e => e.Id, f => Guid.NewGuid())
            .RuleFor(e => e.Name, f => f.Company.CompanyName())
            .RuleFor(e => e.LegalAddress, f => f.Address.FullAddress())
            .RuleFor(e => e.TimeZone, f => null)
            .RuleFor(e => e.Managers, f => new List<Manager>());

        return enterpriseFaker.Generate(count);
    }
}