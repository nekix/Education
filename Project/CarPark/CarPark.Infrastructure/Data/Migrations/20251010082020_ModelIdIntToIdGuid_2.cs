using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModelIdIntToIdGuid_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_model_model_id",
                table: "vehicle");

            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_model_new_guid_model_guid_id",
                table: "vehicle");

            migrationBuilder.DropIndex(
                name: "ix_vehicle_model_id",
                table: "vehicle");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_model_guid_id",
                table: "model");

            migrationBuilder.DropPrimaryKey(
                name: "pk_model",
                table: "model");

            migrationBuilder.DropColumn(
                name: "model_id",
                table: "vehicle");

            migrationBuilder.RenameColumn(
                name: "new_guid_model_guid_id",
                table: "vehicle",
                newName: "model_guid_id");

            migrationBuilder.RenameIndex(
                name: "ix_vehicle_new_guid_model_guid_id",
                table: "vehicle",
                newName: "ix_vehicle_model_guid_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_model",
                table: "model",
                column: "guid_id");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_model_model_guid_id",
                table: "vehicle",
                column: "model_guid_id",
                principalTable: "model",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_model_model_guid_id",
                table: "vehicle");

            migrationBuilder.DropPrimaryKey(
                name: "pk_model",
                table: "model");

            migrationBuilder.RenameColumn(
                name: "model_guid_id",
                table: "vehicle",
                newName: "new_guid_model_guid_id");

            migrationBuilder.RenameIndex(
                name: "ix_vehicle_model_guid_id",
                table: "vehicle",
                newName: "ix_vehicle_new_guid_model_guid_id");

            migrationBuilder.AddColumn<int>(
                name: "model_id",
                table: "vehicle",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "ak_model_guid_id",
                table: "model",
                column: "guid_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_model",
                table: "model",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_model_id",
                table: "vehicle",
                column: "model_id");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_model_model_id",
                table: "vehicle",
                column: "model_id",
                principalTable: "model",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_model_new_guid_model_guid_id",
                table: "vehicle",
                column: "new_guid_model_guid_id",
                principalTable: "model",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
