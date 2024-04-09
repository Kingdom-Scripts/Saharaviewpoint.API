using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saharaviewpoint.API.Migrations
{
    /// <inheritdoc />
    public partial class Eleven : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskAttachments_Tasks_SvpTaskId",
                schema: "dbo",
                table: "TaskAttachments");

            migrationBuilder.DropIndex(
                name: "IX_TaskAttachments_SvpTaskId",
                schema: "dbo",
                table: "TaskAttachments");

            migrationBuilder.DropColumn(
                name: "SvpTaskId",
                schema: "dbo",
                table: "TaskAttachments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SvpTaskId",
                schema: "dbo",
                table: "TaskAttachments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskAttachments_SvpTaskId",
                schema: "dbo",
                table: "TaskAttachments",
                column: "SvpTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskAttachments_Tasks_SvpTaskId",
                schema: "dbo",
                table: "TaskAttachments",
                column: "SvpTaskId",
                principalSchema: "dbo",
                principalTable: "Tasks",
                principalColumn: "Id");
        }
    }
}
