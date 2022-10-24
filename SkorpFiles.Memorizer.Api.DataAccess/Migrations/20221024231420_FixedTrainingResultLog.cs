using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    public partial class FixedTrainingResultLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_jEventLog_rQuestion_QuestionId",
                schema: "memorizer",
                table: "jEventLog");

            migrationBuilder.DropIndex(
                name: "IX_jEventLog_QuestionId",
                schema: "memorizer",
                table: "jEventLog");

            migrationBuilder.DropColumn(
                name: "EventQuestionId",
                schema: "memorizer",
                table: "jEventLog");

            migrationBuilder.DropColumn(
                name: "EventQuestionIsNew",
                schema: "memorizer",
                table: "jEventLog");

            migrationBuilder.DropColumn(
                name: "EventResultPenaltyPoints",
                schema: "memorizer",
                table: "jEventLog");

            migrationBuilder.DropColumn(
                name: "EventResultRating",
                schema: "memorizer",
                table: "jEventLog");

            migrationBuilder.DropColumn(
                name: "EventTypedAnswers",
                schema: "memorizer",
                table: "jEventLog");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                schema: "memorizer",
                table: "jEventLog");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EventQuestionId",
                schema: "memorizer",
                table: "jEventLog",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EventQuestionIsNew",
                schema: "memorizer",
                table: "jEventLog",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventResultPenaltyPoints",
                schema: "memorizer",
                table: "jEventLog",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventResultRating",
                schema: "memorizer",
                table: "jEventLog",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EventTypedAnswers",
                schema: "memorizer",
                table: "jEventLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "QuestionId",
                schema: "memorizer",
                table: "jEventLog",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_jEventLog_QuestionId",
                schema: "memorizer",
                table: "jEventLog",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_jEventLog_rQuestion_QuestionId",
                schema: "memorizer",
                table: "jEventLog",
                column: "QuestionId",
                principalSchema: "memorizer",
                principalTable: "rQuestion",
                principalColumn: "QuestionId");
        }
    }
}
