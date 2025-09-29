using CarPark.Enterprises;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Data.Interfaces;

public interface IEnterprisesDbSet : IDisposable
{
    DbSet<Enterprise> Enterprises { get; }
}