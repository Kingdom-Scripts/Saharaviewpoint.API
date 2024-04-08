using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saharaviewpoint.API.Migrations
{
    /// <inheritdoc />
    public partial class Seven : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FolderNames",
                schema: "dbo",
                table: "Projects",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                schema: "dbo",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                schema: "dbo",
                table: "Projects",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UpdatedById",
                schema: "dbo",
                table: "Projects",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_UpdatedById",
                schema: "dbo",
                table: "Projects",
                column: "UpdatedById",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_UpdatedById",
                schema: "dbo",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_UpdatedById",
                schema: "dbo",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                schema: "dbo",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                schema: "dbo",
                table: "Projects");

            migrationBuilder.AlterColumn<string>(
                name: "FolderNames",
                schema: "dbo",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }
    }
}
