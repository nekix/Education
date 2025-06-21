using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEnterpriseAndDriver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "enterprise_id",
                table: "vehicle",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "enterprise",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    legal_address = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_enterprise", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "driver",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    enterprise_id = table.Column<int>(type: "integer", nullable: false),
                    full_name = table.Column<string>(type: "text", nullable: false),
                    driver_license_number = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver", x => x.id);
                    table.ForeignKey(
                        name: "fk_driver_enterprise_enterprise_id",
                        column: x => x.enterprise_id,
                        principalTable: "enterprise",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "driver_vehicle_assignment",
                columns: table => new
                {
                    driver_id = table.Column<int>(type: "integer", nullable: false),
                    vehicle_id = table.Column<int>(type: "integer", nullable: false),
                    is_active_assignment = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_vehicle_assignment", x => new { x.driver_id, x.vehicle_id });
                    table.ForeignKey(
                        name: "fk_driver_vehicle_assignment_driver_driver_id",
                        column: x => x.driver_id,
                        principalTable: "driver",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_driver_vehicle_assignment_vehicle_vehicle_id",
                        column: x => x.vehicle_id,
                        principalTable: "vehicle",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_enterprise_id",
                table: "vehicle",
                column: "enterprise_id");

            migrationBuilder.CreateIndex(
                name: "ix_driver_enterprise_id",
                table: "driver",
                column: "enterprise_id");

            migrationBuilder.CreateIndex(
                name: "ix_driver_vehicle_assignment_driver_id_is_active_assignment",
                table: "driver_vehicle_assignment",
                columns: new[] { "driver_id", "is_active_assignment" },
                unique: true,
                filter: "is_active_assignment = TRUE");

            migrationBuilder.CreateIndex(
                name: "ix_driver_vehicle_assignment_vehicle_id_is_active_assignment",
                table: "driver_vehicle_assignment",
                columns: new[] { "vehicle_id", "is_active_assignment" },
                unique: true,
                filter: "is_active_assignment = TRUE");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_enterprise_enterprise_id",
                table: "vehicle",
                column: "enterprise_id",
                principalTable: "enterprise",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_enterprise_enterprise_id",
                table: "vehicle");

            migrationBuilder.DropTable(
                name: "driver_vehicle_assignment");

            migrationBuilder.DropTable(
                name: "driver");

            migrationBuilder.DropTable(
                name: "enterprise");

            migrationBuilder.DropIndex(
                name: "ix_vehicle_enterprise_id",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "enterprise_id",
                table: "vehicle");
        }
    }
}
