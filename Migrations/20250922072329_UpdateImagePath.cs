using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMSys.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImagePath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SymptomDescription",
                table: "TroubleshootingViews",
                newName: "SymptomName");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "StepView",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "DiagnosisSteps",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "StepView");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "DiagnosisSteps");

            migrationBuilder.RenameColumn(
                name: "SymptomName",
                table: "TroubleshootingViews",
                newName: "SymptomDescription");
        }
    }
}
