using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    public partial class AddedTrainingResultForeignKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TrainingResultUserId",
                schema: "memorizer",
                table: "jTrainingResult",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_jTrainingResult_TrainingResultQuestionId",
                schema: "memorizer",
                table: "jTrainingResult",
                column: "TrainingResultQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_jTrainingResult_TrainingResultUserId",
                schema: "memorizer",
                table: "jTrainingResult",
                column: "TrainingResultUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_jTrainingResult_AspNetUsers_TrainingResultUserId",
                schema: "memorizer",
                table: "jTrainingResult",
                column: "TrainingResultUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_jTrainingResult_rQuestion_TrainingResultQuestionId",
                schema: "memorizer",
                table: "jTrainingResult",
                column: "TrainingResultQuestionId",
                principalSchema: "memorizer",
                principalTable: "rQuestion",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_jTrainingResult_AspNetUsers_TrainingResultUserId",
                schema: "memorizer",
                table: "jTrainingResult");

            migrationBuilder.DropForeignKey(
                name: "FK_jTrainingResult_rQuestion_TrainingResultQuestionId",
                schema: "memorizer",
                table: "jTrainingResult");

            migrationBuilder.DropIndex(
                name: "IX_jTrainingResult_TrainingResultQuestionId",
                schema: "memorizer",
                table: "jTrainingResult");

            migrationBuilder.DropIndex(
                name: "IX_jTrainingResult_TrainingResultUserId",
                schema: "memorizer",
                table: "jTrainingResult");

            migrationBuilder.AlterColumn<string>(
                name: "TrainingResultUserId",
                schema: "memorizer",
                table: "jTrainingResult",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
