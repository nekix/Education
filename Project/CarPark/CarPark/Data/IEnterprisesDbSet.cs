using CarPark.Models.Enterprises;
using Microsoft.EntityFrameworkCore;

namespace CarPark.Data;

public interface IEnterprisesDbSet : IDisposable
{
    DbSet<Enterprise> Enterprises { get; }
}