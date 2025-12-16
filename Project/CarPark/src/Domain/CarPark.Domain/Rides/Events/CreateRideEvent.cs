using CarPark.DateTimes;

namespace CarPark.Rides.Events;

public readonly record struct CreateRideEvent(Guid Rideid, Guid EnterpriseId, Guid VehicleId, UtcDateTimeOffset StartTime, UtcDateTimeOffset EndTime);