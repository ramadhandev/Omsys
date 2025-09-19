using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OMSys.Migrations
{
    /// <inheritdoc />
    public partial class initialOk : Migration
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

            migrationBuilder.AlterColumn<int>(
                name: "StepId",
                table: "StepResults",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SymptomId",
                table: "Solutions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "SymptomId",
                table: "DiagnosisSteps",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "UnitId",
                table: "Components",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Components_Units_UnitId",
                table: "Components",
                column: "UnitId",
                principalTable: "Units",
                principalColumn: "UnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_DiagnosisSteps_Symptoms_SymptomId",
                table: "DiagnosisSteps",
                column: "SymptomId",
                principalTable: "Symptoms",
                principalColumn: "SymptomId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solutions_Symptoms_SymptomId",
                table: "Solutions",
                column: "SymptomId",
                principalTable: "Symptoms",
                principalColumn: "SymptomId");

            migrationBuilder.AddForeignKey(
                name: "FK_StepResults_DiagnosisSteps_NextStepId",
                table: "StepResults",
                column: "NextStepId",
                principalTable: "DiagnosisSteps",
                principalColumn: "StepId");

            migrationBuilder.AddForeignKey(
                name: "FK_StepResults_Solutions_SolutionId",
                table: "StepResults",
                column: "SolutionId",
                principalTable: "Solutions",
                principalColumn: "SolutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Symptoms_Components_ComponentId",
                table: "Symptoms",
                column: "ComponentId",
                principalTable: "Components",
                principalColumn: "ComponentId");
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

            migrationBuilder.AlterColumn<int>(
                name: "StepId",
                table: "StepResults",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SymptomId",
                table: "Solutions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SymptomId",
                table: "DiagnosisSteps",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UnitId",
                table: "Components",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

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
    }
}
