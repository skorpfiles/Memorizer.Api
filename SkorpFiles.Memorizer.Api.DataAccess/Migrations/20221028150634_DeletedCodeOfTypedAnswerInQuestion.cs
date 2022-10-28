using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    public partial class DeletedCodeOfTypedAnswerInQuestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypedAnswerQuestionCode",
                schema: "memorizer",
                table: "rTypedAnswer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypedAnswerQuestionCode",
                schema: "memorizer",
                table: "rTypedAnswer",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
