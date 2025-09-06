using System.ComponentModel;

namespace CarPark.ViewModels.Enterprises;

public class EnterpriseDetailsViewModel
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    [DisplayName("Legal Address")]
    public required string LegalAddress { get; set; }

    [DisplayName("Time Zone")]
    public required TimeZoneViewModel? TimeZone { get; set; }

    public class TimeZoneViewModel
    {
        public required int Id { get; set; }

        public required string Name { get; set; }
    }
}