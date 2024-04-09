using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saharaviewpoint.API.Migrations
{
    /// <inheritdoc />
    public partial class Ten : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Task_Status",
                schema: "dbo",
                table: "Tasks");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Task_Status",
                schema: "dbo",
                table: "Tasks",
                sql: "[Status] IN ('TO DO', 'IN PROGRESS', 'COMPLETED')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Task_Status",
                schema: "dbo",
                table: "Tasks");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Task_Status",
                schema: "dbo",
                table: "Tasks",
                sql: "[Status] IN ('TODO', 'IN PROGRESS', 'COMPLETED')");
        }
    }
}
