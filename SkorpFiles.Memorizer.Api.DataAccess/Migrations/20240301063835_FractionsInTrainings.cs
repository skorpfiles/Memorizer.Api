using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FractionsInTrainings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TrainingNewQuestionsFraction",
                schema: "memorizer",
                table: "rTraining",
                type: "decimal(4,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TrainingPenaltyQuestionsFraction",
                schema: "memorizer",
                table: "rTraining",
                type: "decimal(4,3)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrainingNewQuestionsFraction",
                schema: "memorizer",
                table: "rTraining");

            migrationBuilder.DropColumn(
                name: "TrainingPenaltyQuestionsFraction",
                schema: "memorizer",
                table: "rTraining");
        }
    }
}
