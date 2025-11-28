namespace CarPark.Controllers.Api.Abstract;

public interface IPaginationMetadata
{
    /// <summary>
    /// Номер страницы
    /// </summary>
    public uint Offset { get; }

    /// <summary>
    /// Размер страницы
    /// </summary>
    public uint Limit { get; }

    /// <summary>
    /// Общее число результатов
    /// </summary>
    public uint Total { get; }
}