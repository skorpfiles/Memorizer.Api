using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NewLabelsArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nnQuestionLabel_sLabel_LabelId",
                schema: "memorizer",
                table: "nnQuestionLabel");

            migrationBuilder.DropTable(
                name: "sLabel",
                schema: "memorizer");

            migrationBuilder.RenameColumn(
                name: "LabelId",
                schema: "memorizer",
                table: "nnQuestionLabel",
                newName: "NormalizedLabelId");

            migrationBuilder.RenameIndex(
                name: "IX_nnQuestionLabel_LabelId",
                schema: "memorizer",
                table: "nnQuestionLabel",
                newName: "IX_nnQuestionLabel_NormalizedLabelId");

            migrationBuilder.AddColumn<string>(
                name: "QuestionLabelName",
                schema: "memorizer",
                table: "nnQuestionLabel",
                type: "nvarchar(max)",
                maxLength: 10000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "rNormalizedLabel",
                schema: "memorizer",
                columns: table => new
                {
                    NormalizedLabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NormalizedLabelName = table.Column<string>(type: "nvarchar(10000)", maxLength: 10000, nullable: false),
                    ObjectCreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rNormalizedLabel", x => x.NormalizedLabelId);
                });

            migrationBuilder.CreateTable(
                name: "nnQuestionnaireLabel",
                schema: "memorizer",
                columns: table => new
                {
                    QuestionnaireLabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NormalizedLabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireLabelName = table.Column<string>(type: "nvarchar(10000)", maxLength: 10000, nullable: false),
                    QuestionnaireLabelIsAlive = table.Column<bool>(type: "bit", nullable: false),
                    ObjectCreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nnQuestionnaireLabel", x => x.QuestionnaireLabelId);
                    table.ForeignKey(
                        name: "FK_nnQuestionnaireLabel_rNormalizedLabel_NormalizedLabelId",
                        column: x => x.NormalizedLabelId,
                        principalSchema: "memorizer",
                        principalTable: "rNormalizedLabel",
                        principalColumn: "NormalizedLabelId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_nnQuestionnaireLabel_rQuestionnaire_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalSchema: "memorizer",
                        principalTable: "rQuestionnaire",
                        principalColumn: "QuestionnaireId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_nnQuestionnaireLabel_NormalizedLabelId",
                schema: "memorizer",
                table: "nnQuestionnaireLabel",
                column: "NormalizedLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_nnQuestionnaireLabel_QuestionnaireId_NormalizedLabelId_QuestionnaireLabelName",
                schema: "memorizer",
                table: "nnQuestionnaireLabel",
                columns: new[] { "QuestionnaireId", "NormalizedLabelId", "QuestionnaireLabelName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rNormalizedLabel_NormalizedLabelName",
                schema: "memorizer",
                table: "rNormalizedLabel",
                column: "NormalizedLabelName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_nnQuestionLabel_rNormalizedLabel_NormalizedLabelId",
                schema: "memorizer",
                table: "nnQuestionLabel",
                column: "NormalizedLabelId",
                principalSchema: "memorizer",
                principalTable: "rNormalizedLabel",
                principalColumn: "NormalizedLabelId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_nnQuestionLabel_rNormalizedLabel_NormalizedLabelId",
                schema: "memorizer",
                table: "nnQuestionLabel");

            migrationBuilder.DropTable(
                name: "nnQuestionnaireLabel",
                schema: "memorizer");

            migrationBuilder.DropTable(
                name: "rNormalizedLabel",
                schema: "memorizer");

            migrationBuilder.DropColumn(
                name: "QuestionLabelName",
                schema: "memorizer",
                table: "nnQuestionLabel");

            migrationBuilder.RenameColumn(
                name: "NormalizedLabelId",
                schema: "memorizer",
                table: "nnQuestionLabel",
                newName: "LabelId");

            migrationBuilder.RenameIndex(
                name: "IX_nnQuestionLabel_NormalizedLabelId",
                schema: "memorizer",
                table: "nnQuestionLabel",
                newName: "IX_nnQuestionLabel_LabelId");

            migrationBuilder.CreateTable(
                name: "sLabel",
                schema: "memorizer",
                columns: table => new
                {
                    LabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LabelCode = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObjectCreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ObjectIsRemoved = table.Column<bool>(type: "bit", nullable: false),
                    ObjectRemovalTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sLabel", x => x.LabelId);
                    table.ForeignKey(
                        name: "FK_sLabel_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_sLabel_OwnerId",
                schema: "memorizer",
                table: "sLabel",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_nnQuestionLabel_sLabel_LabelId",
                schema: "memorizer",
                table: "nnQuestionLabel",
                column: "LabelId",
                principalSchema: "memorizer",
                principalTable: "sLabel",
                principalColumn: "LabelId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
