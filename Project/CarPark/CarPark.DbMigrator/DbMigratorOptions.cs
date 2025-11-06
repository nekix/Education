using System.ComponentModel.DataAnnotations;

namespace CarPark.DbMigrator;

public class DbMigratorOptions
{
    public const string Key = "DbMigrator";

    [Required] 
    public string ConnectionString { get; set; } = default!;
}