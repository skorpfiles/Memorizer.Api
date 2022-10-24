using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    public partial class AddedTrainingResultDbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "jTrainingResult",
                schema: "memorizer",
                columns: table => new
                {
                    TrainingResultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingResultRecordingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrainingResultUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrainingResultQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingResultQuestionHasBeenNew = table.Column<bool>(type: "bit", nullable: false),
                    TrainingResultUntypedAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrainingResultAnswerIsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    TrainingResultRating = table.Column<int>(type: "int", nullable: false),
                    TrainingResultPenaltyPoints = table.Column<int>(type: "int", nullable: false),
                    TrainingResultTimeMilliseconds = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jTrainingResult", x => x.TrainingResultId);
                });

            migrationBuilder.CreateTable(
                name: "jTrainingResultTypedAnswer",
                schema: "memorizer",
                columns: table => new
                {
                    TrtaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingResultId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrtaAnswer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrtaIsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jTrainingResultTypedAnswer", x => x.TrtaId);
                    table.ForeignKey(
                        name: "FK_jTrainingResultTypedAnswer_jTrainingResult_TrainingResultId",
                        column: x => x.TrainingResultId,
                        principalSchema: "memorizer",
                        principalTable: "jTrainingResult",
                        principalColumn: "TrainingResultId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_jTrainingResultTypedAnswer_TrainingResultId",
                schema: "memorizer",
                table: "jTrainingResultTypedAnswer",
                column: "TrainingResultId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "jTrainingResultTypedAnswer",
                schema: "memorizer");

            migrationBuilder.DropTable(
                name: "jTrainingResult",
                schema: "memorizer");
        }
    }
}
