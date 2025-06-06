using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "model_id",
                table: "vehicle",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "model",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    model_name = table.Column<string>(type: "text", nullable: false),
                    vehicle_type = table.Column<string>(type: "text", nullable: false),
                    seats_count = table.Column<int>(type: "integer", nullable: false),
                    max_loading_weight_kg = table.Column<double>(type: "double precision", nullable: false),
                    engine_power_kw = table.Column<double>(type: "double precision", nullable: false),
                    transmission_type = table.Column<string>(type: "text", nullable: false),
                    fuel_system_type = table.Column<string>(type: "text", nullable: false),
                    fuel_tank_volume_liters = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_model", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_model_id",
                table: "vehicle",
                column: "model_id");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_model_model_id",
                table: "vehicle",
                column: "model_id",
                principalTable: "model",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_model_model_id",
                table: "vehicle");

            migrationBuilder.DropTable(
                name: "model");

            migrationBuilder.DropIndex(
                name: "ix_vehicle_model_id",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "model_id",
                table: "vehicle");
        }
    }
}
