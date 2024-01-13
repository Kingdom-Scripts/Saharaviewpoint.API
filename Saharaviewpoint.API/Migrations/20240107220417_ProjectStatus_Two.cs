using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saharaviewpoint.API.Migrations;

/// <inheritdoc />
public partial class ProjectStatus_Two : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropCheckConstraint(
            name: "CK_Project_Status",
            schema: "dbo",
            table: "Projects");

        migrationBuilder.AddCheckConstraint(
            name: "CK_Project_Status",
            schema: "dbo",
            table: "Projects",
            sql: "[Status] IN ('Requested', 'In Progress', 'Completed')");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropCheckConstraint(
            name: "CK_Project_Status",
            schema: "dbo",
            table: "Projects");

        migrationBuilder.AddCheckConstraint(
            name: "CK_Project_Status",
            schema: "dbo",
            table: "Projects",
            sql: "[Status] IN ('Requested', 'InProgress', 'Completed')");
    }
}
