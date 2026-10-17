using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingLabels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TrainingShouldBeAllLabelsInQuestion",
                schema: "memorizer",
                table: "rTraining",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "nnTrainingLabel",
                schema: "memorizer",
                columns: table => new
                {
                    TrainingLabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NormalizedLabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrainingLabelName = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ObjectCreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nnTrainingLabel", x => x.TrainingLabelId);
                    table.ForeignKey(
                        name: "FK_nnTrainingLabel_rNormalizedLabel_NormalizedLabelId",
                        column: x => x.NormalizedLabelId,
                        principalSchema: "memorizer",
                        principalTable: "rNormalizedLabel",
                        principalColumn: "NormalizedLabelId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_nnTrainingLabel_rTraining_TrainingId",
                        column: x => x.TrainingId,
                        principalSchema: "memorizer",
                        principalTable: "rTraining",
                        principalColumn: "TrainingId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_nnTrainingLabel_NormalizedLabelId",
                schema: "memorizer",
                table: "nnTrainingLabel",
                column: "NormalizedLabelId");

            migrationBuilder.CreateIndex(
                name: "IX_nnTrainingLabel_TrainingId_NormalizedLabelId",
                schema: "memorizer",
                table: "nnTrainingLabel",
                columns: new[] { "TrainingId", "NormalizedLabelId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "nnTrainingLabel",
                schema: "memorizer");

            migrationBuilder.DropColumn(
                name: "TrainingShouldBeAllLabelsInQuestion",
                schema: "memorizer",
                table: "rTraining");
        }
    }
}
