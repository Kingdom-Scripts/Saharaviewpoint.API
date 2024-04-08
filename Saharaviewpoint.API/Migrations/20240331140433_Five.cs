using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saharaviewpoint.API.Migrations
{
    /// <inheritdoc />
    public partial class Five : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                schema: "dbo",
                table: "PMInvitations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PMInvitations_CreatedById",
                schema: "dbo",
                table: "PMInvitations",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_PMInvitations_Users_CreatedById",
                schema: "dbo",
                table: "PMInvitations",
                column: "CreatedById",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PMInvitations_Users_CreatedById",
                schema: "dbo",
                table: "PMInvitations");

            migrationBuilder.DropIndex(
                name: "IX_PMInvitations_CreatedById",
                schema: "dbo",
                table: "PMInvitations");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                schema: "dbo",
                table: "PMInvitations");
        }
    }
}
