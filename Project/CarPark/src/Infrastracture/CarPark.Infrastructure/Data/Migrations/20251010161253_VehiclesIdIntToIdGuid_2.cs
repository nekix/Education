using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class VehiclesIdIntToIdGuid_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_geo_time_point_end_geo_time_point_id",
                table: "ride");

            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_geo_time_point_start_geo_time_point_id",
                table: "ride");

            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_vehicle_id",
                table: "ride");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "vehicle",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn);

            migrationBuilder.AddForeignKey(
                name: "fk_ride_vehicle_geo_time_point_end_geo_time_point_id",
                table: "ride",
                column: "end_geo_time_point_id",
                principalTable: "vehicle_geo_time_point",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ride_vehicle_geo_time_point_start_geo_time_point_id",
                table: "ride",
                column: "start_geo_time_point_id",
                principalTable: "vehicle_geo_time_point",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_ride_vehicle_vehicle_id",
                table: "ride",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_geo_time_point_end_geo_time_point_id",
                table: "ride");

            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_geo_time_point_start_geo_time_point_id",
                table: "ride");

            migrationBuilder.DropForeignKey(
                name: "fk_ride_vehicle_vehicle_id",
                table: "ride");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "vehicle",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "fk_ride_vehicle_geo_time_point_end_geo_time_point_id",
                table: "ride",
                column: "end_geo_time_point_id",
                principalTable: "vehicle_geo_time_point",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ride_vehicle_geo_time_point_start_geo_time_point_id",
                table: "ride",
                column: "start_geo_time_point_id",
                principalTable: "vehicle_geo_time_point",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_ride_vehicle_vehicle_id",
                table: "ride",
                column: "vehicle_id",
                principalTable: "vehicle",
                principalColumn: "id");
        }
    }
}
