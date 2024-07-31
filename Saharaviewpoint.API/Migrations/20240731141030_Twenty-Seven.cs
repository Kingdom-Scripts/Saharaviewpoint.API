using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saharaviewpoint.API.Migrations
{
    /// <inheritdoc />
    public partial class TwentySeven : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Document_Type",
                schema: "dbo",
                table: "Documents");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Document_Type",
                schema: "dbo",
                table: "Documents",
                sql: "[Type] IN ('Image', 'PDF', 'Word', 'EXCEL', 'Video', 'Unknown')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Document_Type",
                schema: "dbo",
                table: "Documents");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Document_Type",
                schema: "dbo",
                table: "Documents",
                sql: "[Type] IN ('Image', 'PDF', 'Word', 'EXCEL', 'Unknown')");
        }
    }
}
