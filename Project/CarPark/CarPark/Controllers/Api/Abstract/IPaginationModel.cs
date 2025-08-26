namespace CarPark.Controllers.Api.Abstract;

public interface IPaginationModel<out TData, out TMetadata> where TMetadata : IPaginationMetadata
{
    public TMetadata Meta { get; }

    public IEnumerable<TData> Data { get; }
}