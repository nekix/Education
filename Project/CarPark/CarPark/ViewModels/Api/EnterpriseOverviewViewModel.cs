using System.Text.Json.Serialization;

namespace CarPark.ViewModels.Api;

public class EnterpriseOverviewViewModel
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required string LegalAddress { get; set; }

    public required RelatedEntitiesViewModel RelatedEntities { get; set; }

    public class RelatedEntitiesViewModel
    {
        public required List<int> VehiclesIds { get; set; }

        public required List<int> DriversIds { get; set; }
    }
}