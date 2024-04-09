using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saharaviewpoint.API.Migrations
{
    /// <inheritdoc />
    public partial class Nine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_ReporterId",
                schema: "dbo",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_ReporterId",
                schema: "dbo",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DateCreated",
                schema: "dbo",
                table: "UserRoles");

            migrationBuilder.DropColumn(
                name: "ReporterId",
                schema: "dbo",
                table: "Tasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreated",
                schema: "dbo",
                table: "UserRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ReporterId",
                schema: "dbo",
                table: "Tasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ReporterId",
                schema: "dbo",
                table: "Tasks",
                column: "ReporterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_ReporterId",
                schema: "dbo",
                table: "Tasks",
                column: "ReporterId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
