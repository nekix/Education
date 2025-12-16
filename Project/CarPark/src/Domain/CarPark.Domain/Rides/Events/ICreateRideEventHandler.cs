namespace CarPark.Rides.Events;

public interface ICreateRideEventHandler
{
    public Task Handle(CreateRideEvent @event);
}