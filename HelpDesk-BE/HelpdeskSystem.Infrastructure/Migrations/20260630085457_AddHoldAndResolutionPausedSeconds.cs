using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpdeskSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHoldAndResolutionPausedSeconds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPausedSeconds",
                table: "Tickets",
                newName: "PostResolutionPausedSeconds");

            migrationBuilder.AddColumn<long>(
                name: "OnHoldPausedSeconds",
                table: "Tickets",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$pihQIr0GZVbe4Si6O0H/betWgvIX1RoD1mYSuM3ZJm8cfL3Aa4Xfy");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$7U6AZMv8KI/wkRh/tI3GRu8r1cJf.6BC3WtDhzQequG/afQP0bziG");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnHoldPausedSeconds",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "PostResolutionPausedSeconds",
                table: "Tickets",
                newName: "TotalPausedSeconds");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$EmUF6m4XFBUlkJnfdUHhzOVgNzOS3wPsC6uNYeSBgKaCnwQUcXwWq");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$FA91cUlKAPAdOo3OwyghqexNvt2T4qOefHu5N8bNZ4vZDo6SQ/zr2");
        }
    }
}
