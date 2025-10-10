using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class ManagerIdIntToIdGuid_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "guid_id",
                table: "manager",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql(@"UPDATE manager SET guid_id = gen_random_uuid();");

            migrationBuilder.AddUniqueConstraint(
                name: "ak_manager_guid_id",
                table: "manager",
                column: "guid_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "ak_manager_guid_id",
                table: "manager");

            migrationBuilder.DropColumn(
                name: "guid_id",
                table: "manager");
        }
    }
}
