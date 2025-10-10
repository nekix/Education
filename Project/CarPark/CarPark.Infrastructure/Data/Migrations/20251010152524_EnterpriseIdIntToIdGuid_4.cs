using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnterpriseIdIntToIdGuid_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_enterprise_manager_enterprise_enterprises_id",
                table: "enterprise_manager");

            migrationBuilder.DropForeignKey(
                name: "fk_driver_enterprise_enterprise_id",
                table: "driver");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_enterprise_enterprise_id",
                table: "vehicle");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_enterprise_guid_id",
                table: "enterprise");

            migrationBuilder.DropPrimaryKey(
                name: "pk_enterprise",
                table: "enterprise");

            migrationBuilder.DropColumn(
                name: "id",
                table: "enterprise");

            migrationBuilder.AddPrimaryKey(
                name: "pk_enterprise",
                table: "enterprise",
                column: "guid_id");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_manager_enterprise_enterprises_id",
                table: "enterprise_manager",
                column: "enterprises_id",
                principalTable: "enterprise",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_driver_enterprise_enterprise_id",
                table: "driver",
                column: "enterprise_id",
                principalTable: "enterprise",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_enterprise_enterprise_id",
                table: "vehicle",
                column: "enterprise_guid_id",
                principalTable: "enterprise",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_enterprise_manager_enterprise_enterprises_id",
                table: "enterprise_manager");

            migrationBuilder.DropForeignKey(
                name: "fk_driver_enterprise_enterprise_id",
                table: "driver");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_enterprise_enterprise_id",
                table: "vehicle");

            migrationBuilder.DropPrimaryKey(
                name: "pk_enterprise",
                table: "enterprise");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "enterprise",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddUniqueConstraint(
                name: "ak_enterprise_guid_id",
                table: "enterprise",
                column: "guid_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_enterprise",
                table: "enterprise",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_manager_enterprise_enterprises_id",
                table: "enterprise_manager",
                column: "enterprises_id",
                principalTable: "enterprise",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_driver_enterprise_enterprise_id",
                table: "driver",
                column: "enterprise_id",
                principalTable: "enterprise",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_enterprise_enterprise_id",
                table: "vehicle",
                column: "enterprise_guid_id",
                principalTable: "enterprise",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
