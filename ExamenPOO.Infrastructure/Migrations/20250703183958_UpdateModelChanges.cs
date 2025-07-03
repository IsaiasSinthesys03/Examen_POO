using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExamenPOO.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CourseName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreditHours = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Prerequisites = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Credits = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Career = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SemesterEnrollments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    MaxCredits = table.Column<int>(type: "int", nullable: false, defaultValue: 21),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SemesterEnrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SemesterEnrollments_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnrolledCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SemesterEnrollmentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    EnrollmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Grade = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    GradeDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDropped = table.Column<bool>(type: "bit", nullable: false),
                    DropDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CourseId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrolledCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EnrolledCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EnrolledCourses_Courses_CourseId1",
                        column: x => x.CourseId1,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EnrolledCourses_SemesterEnrollments_SemesterEnrollmentId",
                        column: x => x.SemesterEnrollmentId,
                        principalTable: "SemesterEnrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "CourseCode", "CourseName", "CreatedAt", "CreditHours", "Credits", "Department", "Description", "IsActive", "Name", "Prerequisites", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "CS101", "", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, 4, "Computer Science", "Basic programming concepts using C# and object-oriented programming principles", true, "Introduction to Programming", null, null },
                    { 2, "CS201", "", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, 4, "Computer Science", "Fundamental data structures and algorithmic problem-solving techniques", true, "Data Structures and Algorithms", null, null },
                    { 3, "CS301", "", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, 3, "Computer Science", "Design and implementation of relational database systems", true, "Database Systems", null, null },
                    { 4, "CS401", "", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, 4, "Computer Science", "Software development methodologies and project management", true, "Software Engineering", null, null },
                    { 5, "CS501", "", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, 3, "Computer Science", "Modern web development using ASP.NET Core and related technologies", true, "Web Development", null, null },
                    { 6, "MATH201", "", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, 3, "Mathematics", "Mathematical foundations for computer science", true, "Discrete Mathematics", null, null }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "Id", "Address", "BirthDate", "Career", "CreatedAt", "Email", "FirstName", "IsActive", "LastName", "PasswordHash", "PhoneNumber", "StudentNumber", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Computer Science", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "john.doe@university.edu", "John", true, "Doe", "$2a$11$8ZYdkl8SJ.KoGxKO.4dXQ.qkn7nZfQ7qkn7nZfQ7qkn7nZfQ7qkn7nZfQ", null, "2024001", null },
                    { 2, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Information Systems", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "jane.smith@university.edu", "Jane", true, "Smith", "$2a$11$8ZYdkl8SJ.KoGxKO.4dXQ.qkn7nZfQ7qkn7nZfQ7qkn7nZfQ7qkn7nZfQ", null, "2024002", null },
                    { 3, null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Software Engineering", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "mike.johnson@university.edu", "Mike", true, "Johnson", "$2a$11$8ZYdkl8SJ.KoGxKO.4dXQ.qkn7nZfQ7qkn7nZfQ7qkn7nZfQ7qkn7nZfQ", null, "2024003", null }
                });

            migrationBuilder.InsertData(
                table: "SemesterEnrollments",
                columns: new[] { "Id", "CreatedAt", "EndDate", "EnrollmentDate", "IsActive", "MaxCredits", "Semester", "StartDate", "StudentId", "UpdatedAt", "Year" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 7, 3, 18, 39, 58, 358, DateTimeKind.Utc).AddTicks(7338), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 18, "Fall", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null, 2024 },
                    { 2, new DateTime(2025, 7, 3, 18, 39, 58, 358, DateTimeKind.Utc).AddTicks(8603), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 21, "Fall", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, null, 2024 },
                    { 3, new DateTime(2025, 7, 3, 18, 39, 58, 358, DateTimeKind.Utc).AddTicks(8607), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, 15, "Fall", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, null, 2024 }
                });

            migrationBuilder.InsertData(
                table: "EnrolledCourses",
                columns: new[] { "Id", "CourseId", "CourseId1", "DropDate", "EnrollmentDate", "Grade", "GradeDate", "IsDropped", "SemesterEnrollmentId" },
                values: new object[,]
                {
                    { 1, 1, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, 1 },
                    { 2, 2, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, 1 },
                    { 3, 3, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, 1 },
                    { 4, 6, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, 1 },
                    { 5, 4, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, 1 },
                    { 6, 1, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, 2 },
                    { 7, 3, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, 2 },
                    { 8, 5, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, 2 },
                    { 9, 4, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, 2 },
                    { 10, 1, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, 3 },
                    { 11, 5, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, 3 },
                    { 12, 4, null, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_CourseCode",
                table: "Courses",
                column: "CourseCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EnrolledCourses_CourseId",
                table: "EnrolledCourses",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolledCourses_CourseId1",
                table: "EnrolledCourses",
                column: "CourseId1");

            migrationBuilder.CreateIndex(
                name: "IX_EnrolledCourses_SemesterEnrollmentId_CourseId",
                table: "EnrolledCourses",
                columns: new[] { "SemesterEnrollmentId", "CourseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SemesterEnrollments_StudentId_Semester_Year",
                table: "SemesterEnrollments",
                columns: new[] { "StudentId", "Semester", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_Email",
                table: "Students",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_StudentNumber",
                table: "Students",
                column: "StudentNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnrolledCourses");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "SemesterEnrollments");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "IsActive", "LastName", "PasswordHash", "UpdatedAt", "Username" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@example.com", "Administrator", true, "User", "$2a$11$8ZYdkl8SJ.KoGxKO.4dXQ.qkn7nZfQ7qkn7nZfQ7qkn7nZfQ7qkn7nZfQ", null, "admin" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "demo@example.com", "Demo", true, "User", "$2a$11$8ZYdkl8SJ.KoGxKO.4dXQ.qkn7nZfQ7qkn7nZfQ7qkn7nZfQ7qkn7nZfQ", null, "demo" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "IsActive", "Name", "Price", "Stock", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, "Electronics", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "High-performance gaming laptop with RTX graphics", true, "Laptop Gaming", 1299.99m, 15, null, 1 },
                    { 2, "Electronics", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ergonomic wireless mouse with RGB lighting", true, "Wireless Mouse", 29.99m, 50, null, 1 },
                    { 3, "Home", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ceramic coffee mug with custom design", true, "Coffee Mug", 12.99m, 100, null, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_UserId",
                table: "Products",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }
    }
}
