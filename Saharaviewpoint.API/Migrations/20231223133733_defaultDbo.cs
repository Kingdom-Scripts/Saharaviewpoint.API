using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saharaviewpoint.API.Migrations;

/// <inheritdoc />
public partial class defaultDbo : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "dbo");

        migrationBuilder.RenameTable(
            name: "Users",
            newName: "Users",
            newSchema: "dbo");

        migrationBuilder.RenameTable(
            name: "UserRoles",
            newName: "UserRoles",
            newSchema: "dbo");

        migrationBuilder.RenameTable(
            name: "Roles",
            newName: "Roles",
            newSchema: "dbo");

        migrationBuilder.RenameTable(
            name: "RefreshTokens",
            newName: "RefreshTokens",
            newSchema: "dbo");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameTable(
            name: "Users",
            schema: "dbo",
            newName: "Users");

        migrationBuilder.RenameTable(
            name: "UserRoles",
            schema: "dbo",
            newName: "UserRoles");

        migrationBuilder.RenameTable(
            name: "Roles",
            schema: "dbo",
            newName: "Roles");

        migrationBuilder.RenameTable(
            name: "RefreshTokens",
            schema: "dbo",
            newName: "RefreshTokens");
    }
}
