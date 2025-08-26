namespace CarPark.Controllers.Api.Abstract;

public interface IPaginationRequest
{
    public uint Limit { get; }

    public uint Offset { get; }
}