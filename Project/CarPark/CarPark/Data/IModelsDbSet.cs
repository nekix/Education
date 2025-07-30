using CarPark.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Data;

public interface IModelsDbSet : IDisposable
{
    DbSet<Model> Models { get; }
}