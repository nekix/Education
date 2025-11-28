using CarPark.Models;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Data.Interfaces;

public interface IModelsDbSet : IDisposable
{
    DbSet<Model> Models { get; }
}