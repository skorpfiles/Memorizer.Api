using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RefactoringLabelsOnlyForQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nnEntityLabel",
                schema: "memorizer");

            migrationBuilder.CreateTable(
                name: "nnQuestionLabel",
                schema: "memorizer",
                columns: table => new
                {
                    QuestionLabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ObjectCreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nnQuestionLabel", x => x.QuestionLabelId);
                    table.ForeignKey(
                        name: "FK_nnQuestionLabel_rQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "memorizer",
                        principalTable: "rQuestion",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_nnQuestionLabel_sLabel_LabelId",
                        column: x => x.LabelId,
                        principalSchema: "memorizer",
                        principalTable: "sLabel",
                        principalColumn: "LabelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_nnQuestionLabel_LabelId",
                schema: "memorizer",
                table: "nnQuestionLabel",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_nnQuestionLabel_QuestionId",
                schema: "memorizer",
                table: "nnQuestionLabel",
                column: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nnQuestionLabel",
                schema: "memorizer");

            migrationBuilder.CreateTable(
                name: "nnEntityLabel",
                schema: "memorizer",
                columns: table => new
                {
                    EntityLabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    LabelNumber = table.Column<int>(type: "int", nullable: false),
                    ObjectCreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ParentLabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nnEntityLabel", x => x.EntityLabelId);
                    table.ForeignKey(
                        name: "FK_nnEntityLabel_rQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "memorizer",
                        principalTable: "rQuestion",
                        principalColumn: "QuestionId");
                    table.ForeignKey(
                        name: "FK_nnEntityLabel_rQuestionnaire_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalSchema: "memorizer",
                        principalTable: "rQuestionnaire",
                        principalColumn: "QuestionnaireId");
                    table.ForeignKey(
                        name: "FK_nnEntityLabel_sLabel_LabelId",
                        column: x => x.LabelId,
                        principalSchema: "memorizer",
                        principalTable: "sLabel",
                        principalColumn: "LabelId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_nnEntityLabel_LabelId",
                schema: "memorizer",
                table: "nnEntityLabel",
                column: "LabelId");

            migrationBuilder.CreateIndex(
                name: "IX_nnEntityLabel_QuestionId",
                schema: "memorizer",
                table: "nnEntityLabel",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_nnEntityLabel_QuestionnaireId",
                schema: "memorizer",
                table: "nnEntityLabel",
                column: "QuestionnaireId");
        }
    }
}
