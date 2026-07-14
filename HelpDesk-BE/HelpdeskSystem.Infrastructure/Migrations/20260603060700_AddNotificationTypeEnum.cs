using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpdeskSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationTypeEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:audit_action", "created,updated,assigned,status_changed,sla_breached")
                .Annotation("Npgsql:Enum:notification_type", "ticket_assigned,status_changed,comment_added,sla_breached,new_ticket_created")
                .Annotation("Npgsql:Enum:priority", "low,medium,high")
                .Annotation("Npgsql:Enum:ticket_status", "open,assigned,in_progress,on_hold,resolved,closed,rejected,cancelled,re_open,cannot_resolve")
                .Annotation("Npgsql:Enum:user_role", "admin,agent,requester")
                .OldAnnotation("Npgsql:Enum:audit_action", "created,updated,assigned,status_changed,sla_breached")
                .OldAnnotation("Npgsql:Enum:priority", "low,medium,high")
                .OldAnnotation("Npgsql:Enum:ticket_status", "open,assigned,in_progress,on_hold,resolved,closed,rejected,cancelled,re_open,cannot_resolve")
                .OldAnnotation("Npgsql:Enum:user_role", "admin,agent,requester");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "Notifications",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Notifications",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$aFwF7gftAgvV.VBkMB4vvuKiJy4yFXxfbfBSh1zrTM2HpVz4HRck.");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$JAeaF3u6PQBvQxjdndoOL.yPnhxeKF9hl1zk2/RfXsnSgi7uCYFky");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:audit_action", "created,updated,assigned,status_changed,sla_breached")
                .Annotation("Npgsql:Enum:priority", "low,medium,high")
                .Annotation("Npgsql:Enum:ticket_status", "open,assigned,in_progress,on_hold,resolved,closed,rejected,cancelled,re_open,cannot_resolve")
                .Annotation("Npgsql:Enum:user_role", "admin,agent,requester")
                .OldAnnotation("Npgsql:Enum:audit_action", "created,updated,assigned,status_changed,sla_breached")
                .OldAnnotation("Npgsql:Enum:notification_type", "ticket_assigned,status_changed,comment_added,sla_breached,new_ticket_created")
                .OldAnnotation("Npgsql:Enum:priority", "low,medium,high")
                .OldAnnotation("Npgsql:Enum:ticket_status", "open,assigned,in_progress,on_hold,resolved,closed,rejected,cancelled,re_open,cannot_resolve")
                .OldAnnotation("Npgsql:Enum:user_role", "admin,agent,requester");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "Notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

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
    }
}
