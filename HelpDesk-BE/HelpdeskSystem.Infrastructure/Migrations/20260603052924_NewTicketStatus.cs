using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpdeskSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NewTicketStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TYPE ticket_status
                RENAME VALUE 'hold' TO 'on_hold';
            ");

            migrationBuilder.Sql(@"
                ALTER TYPE ticket_status
                ADD VALUE IF NOT EXISTS 'cancelled';
            ");

            migrationBuilder.Sql(@"
                ALTER TYPE ticket_status
                ADD VALUE IF NOT EXISTS 're_open';
            ");

            migrationBuilder.Sql(@"
                ALTER TYPE ticket_status
                ADD VALUE IF NOT EXISTS 'cannot_resolve';
            ");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$hF3kLZPfbY21Hq/4pq5UY.OuDWLdh0gEiZveEJAt/IhiHClr458DO");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$ymTZiwX1JkQA9wKiE0HhyuKBM.p4fRLTPIWjtgadL8GKte0zdvUF.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:audit_action", "created,updated,assigned,status_changed,sla_breached")
                .Annotation("Npgsql:Enum:priority", "low,medium,high")
                .Annotation("Npgsql:Enum:ticket_status", "open,assigned,rejected,in_progress,hold,resolved,closed")
                .Annotation("Npgsql:Enum:user_role", "admin,agent,requester")
                .OldAnnotation("Npgsql:Enum:audit_action", "created,updated,assigned,status_changed,sla_breached")
                .OldAnnotation("Npgsql:Enum:priority", "low,medium,high")
                .OldAnnotation("Npgsql:Enum:ticket_status", "open,assigned,in_progress,on_hold,resolved,closed,rejected,cancelled,re_open,cannot_resolve")
                .OldAnnotation("Npgsql:Enum:user_role", "admin,agent,requester");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$sFm2cKA29.B7PwKk3csRiu4hfwCcqj5ZDCTT0ZTY4N0gg79VsbMM6");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$V32yu9sOMjfJUohSmOYf/eZ5iELAlBvdYiYKl4ncX8XEy42HiAwqy");
        }
    }
}
