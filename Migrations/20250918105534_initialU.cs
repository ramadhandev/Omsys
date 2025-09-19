using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMSys.Migrations
{
    /// <inheritdoc />
    public partial class initialU : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ComponentId",
                table: "TroubleshootingViews",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                table: "DiagnosisSteps",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComponentId",
                table: "TroubleshootingViews");

            migrationBuilder.DropColumn(
                name: "Diagnosis",
                table: "DiagnosisSteps");
        }
    }
}
