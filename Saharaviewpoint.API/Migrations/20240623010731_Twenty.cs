using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saharaviewpoint.API.Migrations
{
    /// <inheritdoc />
    public partial class Twenty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Codes_Users_OwnerId",
                schema: "dbo",
                table: "Codes");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                schema: "dbo",
                table: "Codes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Codes_Users_OwnerId",
                schema: "dbo",
                table: "Codes",
                column: "OwnerId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Codes_Users_OwnerId",
                schema: "dbo",
                table: "Codes");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                schema: "dbo",
                table: "Codes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
    }
}
