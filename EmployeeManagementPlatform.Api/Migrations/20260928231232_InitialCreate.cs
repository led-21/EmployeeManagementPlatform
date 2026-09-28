using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeeManagementPlatform.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Position = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Department = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Salary = table.Column<decimal>(type: "TEXT", precision: 18, scale: 2, nullable: false),
                    HireDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Active", "CreatedAt", "Department", "Email", "FirstName", "HireDate", "LastName", "Position", "Salary", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, true, new DateTime(2021, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "sarah.connor@example.com", "Sarah", new DateTime(2021, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Connor", "VP of Engineering", 95000m, null },
                    { 2, true, new DateTime(2022, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "alex.rivera@example.com", "Alex", new DateTime(2022, 1, 10, 0, 0, 0, 0, DateTimeKind.Utc), "Rivera", "Senior Full-Stack Developer", 78000m, null },
                    { 3, true, new DateTime(2021, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Product", "elena.rostova@example.com", "Elena", new DateTime(2021, 7, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rostova", "Lead Product Manager", 82000m, null },
                    { 4, true, new DateTime(2022, 6, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Design", "marcus.vance@example.com", "Marcus", new DateTime(2022, 6, 20, 0, 0, 0, 0, DateTimeKind.Utc), "Vance", "UI/UX Designer", 65000m, null },
                    { 5, true, new DateTime(2020, 11, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Human Resources", "amina.diallo@example.com", "Amina", new DateTime(2020, 11, 5, 0, 0, 0, 0, DateTimeKind.Utc), "Diallo", "HR Director", 75000m, null },
                    { 6, true, new DateTime(2023, 2, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "david.kim@example.com", "David", new DateTime(2023, 2, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Kim", "DevOps Engineer", 74000m, null },
                    { 7, true, new DateTime(2023, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Finance", "sofia.martins@example.com", "Sofia", new DateTime(2023, 5, 2, 0, 0, 0, 0, DateTimeKind.Utc), "Martins", "Financial Analyst", 62000m, null }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CreatedAt", "Department", "Email", "FirstName", "HireDate", "LastName", "Position", "Salary", "UpdatedAt" },
                values: new object[] { 8, new DateTime(2023, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", "liam.chen@example.com", "Liam", new DateTime(2023, 9, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Chen", "Operations Specialist", 55000m, null });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Active", "CreatedAt", "Department", "Email", "FirstName", "HireDate", "LastName", "Position", "Salary", "UpdatedAt" },
                values: new object[,]
                {
                    { 9, true, new DateTime(2022, 8, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Marketing", "olivia.taylor@example.com", "Olivia", new DateTime(2022, 8, 12, 0, 0, 0, 0, DateTimeKind.Utc), "Taylor", "Marketing Manager", 68000m, null },
                    { 10, true, new DateTime(2024, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "james.wilson@example.com", "James", new DateTime(2024, 1, 8, 0, 0, 0, 0, DateTimeKind.Utc), "Wilson", "QA Automation Engineer", 63000m, null },
                    { 11, true, new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Engineering", "lucia.gomez@example.com", "Lucia", new DateTime(2023, 10, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Gomez", "Backend Developer", 71000m, null },
                    { 12, true, new DateTime(2022, 4, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Operations", "ethan.hunt@example.com", "Ethan", new DateTime(2022, 4, 15, 0, 0, 0, 0, DateTimeKind.Utc), "Hunt", "Security Specialist", 80000m, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Active",
                table: "Employees",
                column: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Department",
                table: "Employees",
                column: "Department");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Email",
                table: "Employees",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");
        }
    }
}
