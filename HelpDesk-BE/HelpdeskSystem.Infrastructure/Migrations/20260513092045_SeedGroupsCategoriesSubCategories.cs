using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HelpdeskSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedGroupsCategoriesSubCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "ModifiedAt", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 13, 9, 20, 43, 628, DateTimeKind.Utc).AddTicks(1284), 1, null, null, false, null, null, "IT Support" },
                    { 2, new DateTime(2026, 5, 13, 9, 20, 43, 628, DateTimeKind.Utc).AddTicks(1289), 1, null, null, false, null, null, "HR" }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "PasswordHash" },
                values: new object[] { 0, "$2a$11$KEFUhm0LDXkT0SsBjh16c.BsVoKMSSYQ5Qd/K4Y9/87lXiepU.OHi" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$strNd.pqrINfofXk0A4DZOoao0.eW4mF1zbJrag9cVxD5TKRvp/Uq");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "GroupId", "IsDeleted", "ModifiedAt", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 13, 9, 20, 43, 628, DateTimeKind.Utc).AddTicks(1336), 1, null, null, 1, false, null, null, "Hardware" },
                    { 2, new DateTime(2026, 5, 13, 9, 20, 43, 628, DateTimeKind.Utc).AddTicks(1338), 1, null, null, 1, false, null, null, "Software" },
                    { 3, new DateTime(2026, 5, 13, 9, 20, 43, 628, DateTimeKind.Utc).AddTicks(1341), 1, null, null, 1, false, null, null, "Network" },
                    { 4, new DateTime(2026, 5, 13, 9, 20, 43, 628, DateTimeKind.Utc).AddTicks(1342), 1, null, null, 2, false, null, null, "Payroll" },
                    { 5, new DateTime(2026, 5, 13, 9, 20, 43, 628, DateTimeKind.Utc).AddTicks(1344), 1, null, null, 2, false, null, null, "Onboarding" }
                });

            migrationBuilder.InsertData(
                table: "SubCategories",
                columns: new[] { "Id", "CategoryId", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsDeleted", "ModifiedAt", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 5, 13, 9, 20, 43, 628, DateTimeKind.Utc).AddTicks(1403), 1, null, null, false, null, null, "Laptop" },
                    { 2, 1, new DateTime(2026, 5, 13, 9, 20, 43, 628, DateTimeKind.Utc).AddTicks(1404), 1, null, null, false, null, null, "Printer" },
                    { 3, 2, new DateTime(2026, 5, 13, 9, 20, 43, 628, DateTimeKind.Utc).AddTicks(1406), 1, null, null, false, null, null, "OS Issue" },
                    { 4, 2, new DateTime(2026, 5, 13, 9, 20, 43, 628, DateTimeKind.Utc).AddTicks(1407), 1, null, null, false, null, null, "App Crash" },
                    { 5, 3, new DateTime(2026, 5, 13, 9, 20, 43, 628, DateTimeKind.Utc).AddTicks(1409), 1, null, null, false, null, null, "VPN" },
                    { 6, 3, new DateTime(2026, 5, 13, 9, 20, 43, 628, DateTimeKind.Utc).AddTicks(1414), 1, null, null, false, null, null, "WiFi" },
                    { 7, 4, new DateTime(2026, 5, 13, 9, 20, 43, 628, DateTimeKind.Utc).AddTicks(1416), 1, null, null, false, null, null, "Salary" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SubCategories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "PasswordHash" },
                values: new object[] { 1, "$2a$11$avOXrC8noop4pw1NTzmpDufeJgtFxZxdVHCvRZdf29bVBX8u.fujO" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2a$11$OJ8d22UXv3LpAnCNfmzjK.QVN0HS36CLED1hFYR1d4Uds2oyrpNia");
        }
    }
}
