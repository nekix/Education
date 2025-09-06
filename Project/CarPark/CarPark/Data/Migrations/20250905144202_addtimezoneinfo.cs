using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CarPark.Data.Migrations
{
    /// <inheritdoc />
    public partial class addtimezoneinfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "time_zone_id",
                table: "enterprise",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "time_zone",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    iana_tz_id = table.Column<string>(type: "text", nullable: false),
                    windows_tz_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_time_zone", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_enterprise_time_zone_id",
                table: "enterprise",
                column: "time_zone_id");

            migrationBuilder.AddForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_id",
                table: "enterprise",
                column: "time_zone_id",
                principalTable: "time_zone",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_enterprise_tz_infos_time_zone_id",
                table: "enterprise");

            migrationBuilder.DropTable(
                name: "time_zone");

            migrationBuilder.DropIndex(
                name: "ix_enterprise_time_zone_id",
                table: "enterprise");

            migrationBuilder.DropColumn(
                name: "time_zone_id",
                table: "enterprise");
        }
    }
}
