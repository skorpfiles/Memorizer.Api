using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTrainingResultTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrainingResultUntypedAnswer",
                schema: "memorizer",
                table: "jTrainingResult");

            migrationBuilder.RenameColumn(
                name: "TrainingResultTimeMilliseconds",
                schema: "memorizer",
                table: "jTrainingResult",
                newName: "TrainingResultTimeSeconds");

            migrationBuilder.RenameColumn(
                name: "TrainingResultQuestionHasBeenNew",
                schema: "memorizer",
                table: "jTrainingResult",
                newName: "TrainingResultIsNew");

            migrationBuilder.AddColumn<bool>(
                name: "TrainingResultInitialNewStatus",
                schema: "memorizer",
                table: "jTrainingResult",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TrainingResultInitialPenaltyPoints",
                schema: "memorizer",
                table: "jTrainingResult",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrainingResultInitialRating",
                schema: "memorizer",
                table: "jTrainingResult",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrainingResultInitialNewStatus",
                schema: "memorizer",
                table: "jTrainingResult");

            migrationBuilder.DropColumn(
                name: "TrainingResultInitialPenaltyPoints",
                schema: "memorizer",
                table: "jTrainingResult");

            migrationBuilder.DropColumn(
                name: "TrainingResultInitialRating",
                schema: "memorizer",
                table: "jTrainingResult");

            migrationBuilder.RenameColumn(
                name: "TrainingResultTimeSeconds",
                schema: "memorizer",
                table: "jTrainingResult",
                newName: "TrainingResultTimeMilliseconds");

            migrationBuilder.RenameColumn(
                name: "TrainingResultIsNew",
                schema: "memorizer",
                table: "jTrainingResult",
                newName: "TrainingResultQuestionHasBeenNew");

            migrationBuilder.AddColumn<string>(
                name: "TrainingResultUntypedAnswer",
                schema: "memorizer",
                table: "jTrainingResult",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
