namespace CarPark.ManagersOperations;

public abstract class BaseManagerCommandQuery
{
    public required Guid RequestingManagerId { get; set; }
}