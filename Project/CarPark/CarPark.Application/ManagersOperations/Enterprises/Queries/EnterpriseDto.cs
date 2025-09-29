namespace CarPark.ManagersOperations.Enterprises.Queries;

public class EnterpriseDto
{
    public required int Id { get; set; }

    public required string Name { get; set; }

    public required string LegalAddress { get; set; }

    public required int? TimeZoneId { get; set; }

    public required RelatedEntitiesDto RelatedEntities { get; set; }

    public class RelatedEntitiesDto
    {
        public required List<int> VehiclesIds { get; set; }

        public required List<int> DriversIds { get; set; }
    }
}