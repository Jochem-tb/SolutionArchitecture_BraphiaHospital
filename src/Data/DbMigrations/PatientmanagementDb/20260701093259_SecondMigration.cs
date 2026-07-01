using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.DbMigrations.PatientManagementDb
{
    /// <inheritdoc />
    public partial class SecondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Patient",
                table: "Patient");

            migrationBuilder.RenameTable(
                name: "Patient",
                newName: "Patients");

            migrationBuilder.RenameIndex(
                name: "IX_Patient_InsuranceNumber",
                table: "Patients",
                newName: "IX_Patients_InsuranceNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.RenameTable(
                name: "Patients",
                newName: "Patient");

            migrationBuilder.RenameIndex(
                name: "IX_Patients_InsuranceNumber",
                table: "Patient",
                newName: "IX_Patient_InsuranceNumber");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patient",
                table: "Patient",
                column: "PatientId");
        }
    }
}
