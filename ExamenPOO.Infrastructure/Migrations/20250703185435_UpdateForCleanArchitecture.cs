using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExamenPOO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForCleanArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "SemesterEnrollments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 18, 54, 34, 276, DateTimeKind.Utc).AddTicks(7336));

            migrationBuilder.UpdateData(
                table: "SemesterEnrollments",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 18, 54, 34, 276, DateTimeKind.Utc).AddTicks(8998));

            migrationBuilder.UpdateData(
                table: "SemesterEnrollments",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 18, 54, 34, 276, DateTimeKind.Utc).AddTicks(9001));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Students",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Students",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "SemesterEnrollments",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 18, 39, 58, 358, DateTimeKind.Utc).AddTicks(7338));

            migrationBuilder.UpdateData(
                table: "SemesterEnrollments",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 18, 39, 58, 358, DateTimeKind.Utc).AddTicks(8603));

            migrationBuilder.UpdateData(
                table: "SemesterEnrollments",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2025, 7, 3, 18, 39, 58, 358, DateTimeKind.Utc).AddTicks(8607));

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Address", "BirthDate", "Career", "CreatedAt", "Email", "FirstName", "IsActive", "LastName", "PasswordHash", "PhoneNumber", "StudentNumber", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Computer Science", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "john.doe@university.edu", "John", true, "Doe", "$2a$11$8ZYdkl8SJ.KoGxKO.4dXQ.qkn7nZfQ7qkn7nZfQ7qkn7nZfQ7qkn7nZfQ", null, "2024001", null },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Information Systems", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "jane.smith@university.edu", "Jane", true, "Smith", "$2a$11$8ZYdkl8SJ.KoGxKO.4dXQ.qkn7nZfQ7qkn7nZfQ7qkn7nZfQ7qkn7nZfQ", null, "2024002", null },
                    { 3, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Software Engineering", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "mike.johnson@university.edu", "Mike", true, "Johnson", "$2a$11$8ZYdkl8SJ.KoGxKO.4dXQ.qkn7nZfQ7qkn7nZfQ7qkn7nZfQ7qkn7nZfQ", null, "2024003", null }
                });
        }
    }
}
