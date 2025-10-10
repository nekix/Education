using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnterpriseIdIntToIdGuid_5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "enterprise_guid_id",
                table: "vehicle",
                newName: "enterprise_id");

            migrationBuilder.RenameIndex(
                name: "ix_vehicle_enterprise_guid_id",
                table: "vehicle",
                newName: "ix_vehicle_enterprise_id");

            migrationBuilder.RenameColumn(
                name: "guid_id",
                table: "enterprise",
                newName: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "enterprise_id",
                table: "vehicle",
                newName: "enterprise_guid_id");

            migrationBuilder.RenameIndex(
                name: "ix_vehicle_enterprise_id",
                table: "vehicle",
                newName: "ix_vehicle_enterprise_guid_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "enterprise",
                newName: "guid_id");
        }
    }
}
