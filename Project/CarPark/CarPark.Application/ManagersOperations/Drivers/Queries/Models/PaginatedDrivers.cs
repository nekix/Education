using CarPark.ManagersOperations.Vehicles.Queries.Models;

namespace CarPark.ManagersOperations.Drivers.Queries.Models;

public class PaginatedDrivers : IPaginationModel<DriverDto, PaginatedDrivers.Metadata>
{
    public required Metadata Meta { get; init; }

    public required IEnumerable<DriverDto> Data { get; init; }

    public class Metadata : IPaginationMetadata
    {
        public uint Offset { get; init; }

        public uint Limit { get; init; }

        public uint Total { get; init; }
    }
}