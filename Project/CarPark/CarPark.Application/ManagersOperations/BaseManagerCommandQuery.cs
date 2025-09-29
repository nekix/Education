namespace CarPark.ManagersOperations;

public abstract class BaseManagerCommandQuery
{
    public required int RequestingManagerId { get; set; }
}