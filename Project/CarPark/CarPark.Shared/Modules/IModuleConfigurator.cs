using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarPark.Shared.Modules;

public interface IModuleConfigurator
{
    public void ConfigureModule(IServiceCollection services, IConfiguration configuration);
}