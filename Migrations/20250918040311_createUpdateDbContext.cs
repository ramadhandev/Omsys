using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMSys.Migrations
{
    /// <inheritdoc />
    public partial class createUpdateDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Components_Units_UnitId",
                table: "Components");

            migrationBuilder.DropForeignKey(
                name: "FK_DiagnosisSteps_Symptoms_SymptomId",
                table: "DiagnosisSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Symptoms_SymptomId",
                table: "Solutions");

            migrationBuilder.DropForeignKey(
                name: "FK_StepResults_DiagnosisSteps_NextStepId",
                table: "StepResults");

            migrationBuilder.DropForeignKey(
                name: "FK_StepResults_Solutions_SolutionId",
                table: "StepResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Symptoms_Components_ComponentId",
                table: "Symptoms");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Units_UnitId",
                table: "Components",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "UnitId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DiagnosisSteps_Symptoms_SymptomId",
                table: "DiagnosisSteps",
                column: "SymptomId",
                principalTable: "Symptoms",
                principalColumn: "SymptomId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Symptoms_SymptomId",
                table: "Solutions",
                column: "SymptomId",
                principalTable: "Symptoms",
                principalColumn: "SymptomId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StepResults_DiagnosisSteps_NextStepId",
                table: "StepResults",
                column: "NextStepId",
                principalTable: "DiagnosisSteps",
                principalColumn: "StepId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StepResults_Solutions_SolutionId",
                table: "StepResults",
                column: "SolutionId",
                principalTable: "Solutions",
                principalColumn: "SolutionId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Symptoms_Components_ComponentId",
                table: "Symptoms",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "ComponentId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Components_Units_UnitId",
                table: "Components");

            migrationBuilder.DropForeignKey(
                name: "FK_DiagnosisSteps_Symptoms_SymptomId",
                table: "DiagnosisSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_Solutions_Symptoms_SymptomId",
                table: "Solutions");

            migrationBuilder.DropForeignKey(
                name: "FK_StepResults_DiagnosisSteps_NextStepId",
                table: "StepResults");

            migrationBuilder.DropForeignKey(
                name: "FK_StepResults_Solutions_SolutionId",
                table: "StepResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Symptoms_Components_ComponentId",
                table: "Symptoms");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Units_UnitId",
                table: "Components",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "UnitId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DiagnosisSteps_Symptoms_SymptomId",
                table: "DiagnosisSteps",
                column: "SymptomId",
                principalTable: "Symptoms",
                principalColumn: "SymptomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Symptoms_SymptomId",
                table: "Solutions",
                column: "SymptomId",
                principalTable: "Symptoms",
                principalColumn: "SymptomId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StepResults_DiagnosisSteps_NextStepId",
                table: "StepResults",
                column: "NextStepId",
                principalTable: "DiagnosisSteps",
                principalColumn: "StepId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StepResults_Solutions_SolutionId",
                table: "StepResults",
                column: "SolutionId",
                principalTable: "Solutions",
                principalColumn: "SolutionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Symptoms_Components_ComponentId",
                table: "Symptoms",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "ComponentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
