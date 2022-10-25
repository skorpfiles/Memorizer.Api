using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    public partial class NullForQuestionReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "QuestionReference",
                schema: "memorizer",
                table: "rQuestion",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "QuestionReference",
                schema: "memorizer",
                table: "rQuestion",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
