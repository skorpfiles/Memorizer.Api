using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Trainings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rTraining",
                schema: "memorizer",
                columns: table => new
                {
                    TrainingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TrainingName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrainingLastTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrainingLengthType = table.Column<int>(type: "int", nullable: false),
                    TrainingQuestionsCount = table.Column<int>(type: "int", nullable: false),
                    TrainingTimeMinutes = table.Column<int>(type: "int", nullable: false),
                    ObjectCreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ObjectIsRemoved = table.Column<bool>(type: "bit", nullable: false),
                    ObjectRemovalTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rTraining", x => x.TrainingId);
                    table.ForeignKey(
                        name: "FK_rTraining_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "nnTrainingQuestionnaire",
                schema: "memorizer",
                columns: table => new
                {
                    TrainingQuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObjectCreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nnTrainingQuestionnaire", x => x.TrainingQuestionnaireId);
                    table.ForeignKey(
                        name: "FK_nnTrainingQuestionnaire_rQuestionnaire_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalSchema: "memorizer",
                        principalTable: "rQuestionnaire",
                        principalColumn: "QuestionnaireId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_nnTrainingQuestionnaire_rTraining_TrainingId",
                        column: x => x.TrainingId,
                        principalSchema: "memorizer",
                        principalTable: "rTraining",
                        principalColumn: "TrainingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_nnTrainingQuestionnaire_QuestionnaireId",
                schema: "memorizer",
                table: "nnTrainingQuestionnaire",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_nnTrainingQuestionnaire_TrainingId",
                schema: "memorizer",
                table: "nnTrainingQuestionnaire",
                column: "TrainingId");

            migrationBuilder.CreateIndex(
                name: "IX_rTraining_OwnerId",
                schema: "memorizer",
                table: "rTraining",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nnTrainingQuestionnaire",
                schema: "memorizer");

            migrationBuilder.DropTable(
                name: "rTraining",
                schema: "memorizer");
        }
    }
}
