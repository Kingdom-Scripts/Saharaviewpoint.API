using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saharaviewpoint.API.Migrations
{
    /// <inheritdoc />
    public partial class Six : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_User_Type",
                schema: "dbo",
                table: "Users");

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_Type",
                schema: "dbo",
                table: "Users",
                sql: "[Type] IN ('SVP Official', 'SVP Admin', 'SVP Manager', 'Business Manager', 'Business Client', 'Client')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_User_Type",
                schema: "dbo",
                table: "Users");

            migrationBuilder.AddCheckConstraint(
                name: "CK_User_Type",
                schema: "dbo",
                table: "Users",
                sql: "[Type] IN ('Business', 'Client', 'Manager')");
        }
    }
}
