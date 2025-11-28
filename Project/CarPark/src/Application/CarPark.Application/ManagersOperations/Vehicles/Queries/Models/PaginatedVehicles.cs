namespace CarPark.ManagersOperations.Vehicles.Queries.Models;

public class PaginatedVehicles : IPaginationModel<VehicleDto, PaginatedVehicles.Metadata>
{
    public required Metadata Meta { get; init; }

    public required IEnumerable<VehicleDto> Data { get; init; }

    public class Metadata : IPaginationMetadata
    {
        public uint Offset { get; init; }

        public uint Limit { get; init; }

        public uint Total { get; init; }
    }
}