using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace CarPark
{
    public class InfrastractureModuleOptions
    {
        [Required]
        [ConfigurationKeyName("ConnectionStrings:Default")]
        public required string ConnectionString { get; set; }
    }
}