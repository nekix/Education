namespace CarPark.ManagersOperations.Enterprises.Queries;

public class EnterpriseDto
{
    public required Guid Id { get; set; }

    public required string Name { get; set; }

    public required string LegalAddress { get; set; }

    public required Guid? TimeZoneId { get; set; }

    public required RelatedEntitiesDto RelatedEntities { get; set; }

    public class RelatedEntitiesDto
    {
        public required List<Guid> VehiclesIds { get; set; }

        public required List<Guid> DriversIds { get; set; }
    }
}