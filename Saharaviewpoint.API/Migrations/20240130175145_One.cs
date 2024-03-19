using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saharaviewpoint.API.Migrations
{
    /// <inheritdoc />
    public partial class One : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Username",
                schema: "dbo",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                schema: "dbo",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                schema: "dbo",
                table: "ProjectTypes",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                schema: "dbo",
                table: "Projects",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                schema: "dbo",
                table: "Documents",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "dbo",
                table: "UserRoles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "UserRoles");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "Users",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "ProjectTypes",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "Projects",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                schema: "dbo",
                table: "Documents",
                newName: "DateCreated");

            migrationBuilder.AddColumn<string>(
                name: "Username",
                schema: "dbo",
                table: "Users",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
