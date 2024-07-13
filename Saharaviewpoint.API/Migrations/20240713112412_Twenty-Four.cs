using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saharaviewpoint.API.Migrations
{
    /// <inheritdoc />
    public partial class TwentyFour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompletedById",
                schema: "dbo",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedOn",
                schema: "dbo",
                table: "Projects",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedById",
                schema: "dbo",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "CompletedOn",
                schema: "dbo",
                table: "Projects");
        }
    }
}
