using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saharaviewpoint.API.Migrations
{
    /// <inheritdoc />
    public partial class Sixteen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "dbo",
                table: "Codes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                schema: "dbo",
                table: "Codes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Codes_OwnerId",
                schema: "dbo",
                table: "Codes",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Codes_Users_OwnerId",
                schema: "dbo",
                table: "Codes",
                column: "OwnerId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Codes_Users_OwnerId",
                schema: "dbo",
                table: "Codes");

            migrationBuilder.DropIndex(
                name: "IX_Codes_OwnerId",
                schema: "dbo",
                table: "Codes");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                schema: "dbo",
                table: "Codes");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                schema: "dbo",
                table: "Codes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
