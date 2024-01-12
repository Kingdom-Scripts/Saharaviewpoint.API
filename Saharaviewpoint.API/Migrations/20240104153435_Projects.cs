using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Saharaviewpoint.API.Migrations;

/// <inheritdoc />
public partial class Projects : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Id",
            schema: "dbo",
            table: "UserRoles",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            schema: "dbo",
            table: "Roles",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            schema: "dbo",
            table: "RefreshTokens",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255,
            oldNullable: true);

        migrationBuilder.CreateTable(
            name: "Projects",
            schema: "dbo",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                AssigneeId = table.Column<int>(type: "int", nullable: true),
                Status = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false),
                IsPriority = table.Column<bool>(type: "bit", nullable: false),
                CreatedById = table.Column<int>(type: "int", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                DeletedById = table.Column<int>(type: "int", nullable: true),
                DateDeleted = table.Column<DateTime>(type: "datetime2", nullable: true),
                DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Projects", x => x.Id);
                table.CheckConstraint("CK_Project_Status", "[Status] IN ('Requested', 'In Progress', 'Completed')");
                table.ForeignKey(
                    name: "FK_Projects_Users_AssigneeId",
                    column: x => x.AssigneeId,
                    principalSchema: "dbo",
                    principalTable: "Users",
                    principalColumn: "Id");
                table.ForeignKey(
                    name: "FK_Projects_Users_CreatedById",
                    column: x => x.CreatedById,
                    principalSchema: "dbo",
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Projects_Users_DeletedById",
                    column: x => x.DeletedById,
                    principalSchema: "dbo",
                    principalTable: "Users",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_Projects_AssigneeId",
            schema: "dbo",
            table: "Projects",
            column: "AssigneeId");

        migrationBuilder.CreateIndex(
            name: "IX_Projects_CreatedById",
            schema: "dbo",
            table: "Projects",
            column: "CreatedById");

        migrationBuilder.CreateIndex(
            name: "IX_Projects_DeletedById",
            schema: "dbo",
            table: "Projects",
            column: "DeletedById");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Projects",
            schema: "dbo");

        migrationBuilder.DropColumn(
            name: "Id",
            schema: "dbo",
            table: "UserRoles");

        migrationBuilder.AlterColumn<string>(
            name: "Name",
            schema: "dbo",
            table: "Roles",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "Code",
            schema: "dbo",
            table: "RefreshTokens",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(255)",
            oldMaxLength: 255);
    }
}
