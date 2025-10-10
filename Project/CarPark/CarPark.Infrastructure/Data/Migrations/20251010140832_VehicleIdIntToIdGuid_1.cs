using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class VehicleIdIntToIdGuid_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "driver",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddColumn<Guid>(
                name: "guid_id",
                table: "driver",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql("UPDATE driver SET guid_id = gen_random_uuid();");

            migrationBuilder.AddUniqueConstraint(
                name: "ak_driver_guid_id",
                table: "driver",
                column: "guid_id");

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver",
                column: "assigned_vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_driver_guid_id",
                table: "driver");

            migrationBuilder.DropColumn(
                name: "guid_id",
                table: "driver");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "driver",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "fk_driver_vehicle_assigned_vehicle_id",
                table: "driver",
                column: "assigned_vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id");
        }
    }
}
