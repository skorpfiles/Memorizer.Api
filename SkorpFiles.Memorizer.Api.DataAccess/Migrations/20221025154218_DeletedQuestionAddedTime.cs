using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    public partial class DeletedQuestionAddedTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionAddedTime",
                schema: "memorizer",
                table: "rQuestion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "QuestionAddedTime",
                schema: "memorizer",
                table: "rQuestion",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
