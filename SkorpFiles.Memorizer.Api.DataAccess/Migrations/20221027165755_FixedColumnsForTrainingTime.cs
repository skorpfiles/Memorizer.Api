using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    public partial class FixedColumnsForTrainingTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TypedAnswerQuestionId",
                schema: "memorizer",
                table: "rTypedAnswer",
                newName: "TypedAnswerQuestionCode");

            migrationBuilder.AddColumn<int>(
                name: "QuestionEstimatedTrainingTimeSeconds",
                schema: "memorizer",
                table: "rQuestion",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionEstimatedTrainingTimeSeconds",
                schema: "memorizer",
                table: "rQuestion");

            migrationBuilder.RenameColumn(
                name: "TypedAnswerQuestionCode",
                schema: "memorizer",
                table: "rTypedAnswer",
                newName: "TypedAnswerQuestionId");
        }
    }
}
