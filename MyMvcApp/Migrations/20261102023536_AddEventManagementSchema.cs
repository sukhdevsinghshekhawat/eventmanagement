using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace MyMvcApp.Migrations
{
    /// <inheritdoc />
    public partial class AddEventManagementSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Department",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "EventApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    ApplicantUserId = table.Column<string>(type: "varchar(255)", nullable: false),
                    ApplicantName = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false),
                    Department = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ReviewedByUserId = table.Column<string>(type: "longtext", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ReviewerNotes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventApplications_AspNetUsers_ApplicantUserId",
                        column: x => x.ApplicantUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EventApplications_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EventDepartments",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false),
                    Department = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventDepartments", x => new { x.EventId, x.Department });
                    table.ForeignKey(
                        name: "FK_EventDepartments_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EventApplications_ApplicantUserId",
                table: "EventApplications",
                column: "ApplicantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_EventApplications_EventId_ApplicantUserId",
                table: "EventApplications",
                columns: new[] { "EventId", "ApplicantUserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventApplications");

            migrationBuilder.DropTable(
                name: "EventDepartments");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Events",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }
}
