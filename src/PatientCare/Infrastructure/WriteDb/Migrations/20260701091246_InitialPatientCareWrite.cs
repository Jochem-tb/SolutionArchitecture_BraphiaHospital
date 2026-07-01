using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientCare.Infrastructure.WriteDb.Migrations
{
    /// <inheritdoc />
    public partial class InitialPatientCareWrite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentCache",
                columns: table => new
                {
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhysicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentCache", x => x.AppointmentId);
                });

            migrationBuilder.CreateTable(
                name: "MedicalHistoryEntries",
                columns: table => new
                {
                    EntryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhysicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalHistoryEntries", x => x.EntryId);
                });

            migrationBuilder.CreateTable(
                name: "PatientCache",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientCache", x => x.PatientId);
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    PrescriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicationDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PharmacyNotified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.PrescriptionId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentCache_PatientId",
                table: "AppointmentCache",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentCache_PhysicianId",
                table: "AppointmentCache",
                column: "PhysicianId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistoryEntries_PatientId",
                table: "MedicalHistoryEntries",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistoryEntries_PhysicianId",
                table: "MedicalHistoryEntries",
                column: "PhysicianId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistoryEntries_Timestamp",
                table: "MedicalHistoryEntries",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Prescriptions_PatientId",
                table: "Prescriptions",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentCache");

            migrationBuilder.DropTable(
                name: "MedicalHistoryEntries");

            migrationBuilder.DropTable(
                name: "PatientCache");

            migrationBuilder.DropTable(
                name: "Prescriptions");
        }
    }
}
