using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saharaviewpoint.API.Migrations
{
    /// <inheritdoc />
    public partial class TwentyThree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsFufilled",
                schema: "dbo",
                table: "ProjectTaskApprovals",
                newName: "IsFulfilled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsFulfilled",
                schema: "dbo",
                table: "ProjectTaskApprovals",
                newName: "IsFufilled");
        }
    }
}
