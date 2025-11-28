using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModelIdIntToIdGuid_4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_model_model_guid_id",
                table: "vehicle");

            migrationBuilder.RenameColumn(
                name: "model_guid_id",
                table: "vehicle",
                newName: "model_id");

            migrationBuilder.RenameIndex(
                name: "ix_vehicle_model_guid_id",
                table: "vehicle",
                newName: "ix_vehicle_model_id");

            migrationBuilder.RenameColumn(
                name: "guid_id",
                table: "model",
                newName: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_model_model_id",
                table: "vehicle",
                column: "model_id",
                principalTable: "model",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_model_model_id",
                table: "vehicle");

            migrationBuilder.RenameColumn(
                name: "model_id",
                table: "vehicle",
                newName: "model_guid_id");

            migrationBuilder.RenameIndex(
                name: "ix_vehicle_model_id",
                table: "vehicle",
                newName: "ix_vehicle_model_guid_id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "model",
                newName: "guid_id");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_model_model_guid_id",
                table: "vehicle",
                column: "model_guid_id",
                principalTable: "model",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
