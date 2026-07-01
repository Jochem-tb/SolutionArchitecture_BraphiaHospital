using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PatientCare.Infrastructure.ReadDb.Migrations
{
    /// <inheritdoc />
    public partial class InitialPatientCareRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientDossiers",
                columns: table => new
                {
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDossiers", x => x.PatientId);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentReadItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhysicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentReadItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentReadItems_PatientDossiers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "PatientDossiers",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicalHistoryReadItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PhysicianId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalHistoryReadItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MedicalHistoryReadItems_PatientDossiers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "PatientDossiers",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PrescriptionReadItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MedicationDetails = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PharmacyNotified = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrescriptionReadItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrescriptionReadItems_PatientDossiers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "PatientDossiers",
                        principalColumn: "PatientId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentReadItems_AppointmentId",
                table: "AppointmentReadItems",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentReadItems_PatientId",
                table: "AppointmentReadItems",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistoryReadItems_PatientId",
                table: "MedicalHistoryReadItems",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalHistoryReadItems_Timestamp",
                table: "MedicalHistoryReadItems",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionReadItems_PatientId",
                table: "PrescriptionReadItems",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentReadItems");

            migrationBuilder.DropTable(
                name: "MedicalHistoryReadItems");

            migrationBuilder.DropTable(
                name: "PrescriptionReadItems");

            migrationBuilder.DropTable(
                name: "PatientDossiers");
        }
    }
}
