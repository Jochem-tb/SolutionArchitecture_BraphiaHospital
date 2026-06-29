using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.DbMigrations.MedicalTreatmentDb
{
    /// <inheritdoc />
    public partial class InitialMedicalTreatmentMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MedicalTreatments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhysicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Results = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExaminedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalTreatments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicalHistoryEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhysicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EntryType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalHistoryEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalHistoryEntries_MedicalTreatments_TreatmentId",
                        column: x => x.TreatmentId,
                        principalTable: "MedicalTreatments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prescriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TreatmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PhysicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MedicationDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PharmacyNotified = table.Column<bool>(type: "bit", nullable: false),
                    PrescribedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescriptions_MedicalTreatments_TreatmentId",
                        column: x => x.TreatmentId,
                        principalTable: "MedicalTreatments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(name: "IX_MedicalHistoryEntries_AppointmentId", table: "MedicalHistoryEntries", column: "AppointmentId");
            migrationBuilder.CreateIndex(name: "IX_MedicalHistoryEntries_PatientId", table: "MedicalHistoryEntries", column: "PatientId");
            migrationBuilder.CreateIndex(name: "IX_MedicalHistoryEntries_PhysicianId", table: "MedicalHistoryEntries", column: "PhysicianId");
            migrationBuilder.CreateIndex(name: "IX_MedicalHistoryEntries_TreatmentId", table: "MedicalHistoryEntries", column: "TreatmentId");
            migrationBuilder.CreateIndex(name: "IX_MedicalTreatments_AppointmentId", table: "MedicalTreatments", column: "AppointmentId", unique: true, filter: "[AppointmentId] IS NOT NULL");
            migrationBuilder.CreateIndex(name: "IX_MedicalTreatments_PatientId", table: "MedicalTreatments", column: "PatientId");
            migrationBuilder.CreateIndex(name: "IX_MedicalTreatments_PhysicianId", table: "MedicalTreatments", column: "PhysicianId");
            migrationBuilder.CreateIndex(name: "IX_Prescriptions_AppointmentId", table: "Prescriptions", column: "AppointmentId");
            migrationBuilder.CreateIndex(name: "IX_Prescriptions_PatientId", table: "Prescriptions", column: "PatientId");
            migrationBuilder.CreateIndex(name: "IX_Prescriptions_PhysicianId", table: "Prescriptions", column: "PhysicianId");
            migrationBuilder.CreateIndex(name: "IX_Prescriptions_TreatmentId", table: "Prescriptions", column: "TreatmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "MedicalHistoryEntries");
            migrationBuilder.DropTable(name: "Prescriptions");
            migrationBuilder.DropTable(name: "MedicalTreatments");
        }
    }
}
