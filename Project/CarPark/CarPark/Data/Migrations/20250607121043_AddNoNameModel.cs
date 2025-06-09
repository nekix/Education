using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNoNameModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "model",
                columns: new[] { "id", "engine_power_kw", "fuel_system_type", "fuel_tank_volume_liters", "max_loading_weight_kg", "model_name", "seats_count", "transmission_type", "vehicle_type" },
                values: new object[] { -1, 0.0, "", "", 0.0, "NoName", 0, "", "" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "model",
                keyColumn: "id",
                keyValue: -1);
        }
    }
}
