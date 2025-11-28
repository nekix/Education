using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class ModelIdIntToIdGuid_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "guid_id",
                table: "model",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql(@"UPDATE model SET guid_id = gen_random_uuid();");

            migrationBuilder.AddUniqueConstraint(
                name: "ak_model_guid_id",
                table: "model",
                column: "guid_id");

            migrationBuilder.AddColumn<Guid>(
                name: "new_guid_model_guid_id",
                table: "vehicle",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "ix_vehicle_new_guid_model_guid_id",
                table: "vehicle",
                column: "new_guid_model_guid_id");

            migrationBuilder.Sql(@"UPDATE vehicle SET new_guid_model_guid_id = m.guid_id FROM model m WHERE vehicle.model_id = m.id;");

            migrationBuilder.AddForeignKey(
                name: "fk_vehicle_model_new_guid_model_guid_id",
                table: "vehicle",
                column: "new_guid_model_guid_id",
                principalTable: "model",
                principalColumn: "guid_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_vehicle_model_new_guid_model_guid_id",
                table: "vehicle");

            migrationBuilder.DropIndex(
                name: "ix_vehicle_new_guid_model_guid_id",
                table: "vehicle");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_model_guid_id",
                table: "model");

            migrationBuilder.DropColumn(
                name: "new_guid_model_guid_id",
                table: "vehicle");

            migrationBuilder.DropColumn(
                name: "guid_id",
                table: "model");
        }
    }
}
