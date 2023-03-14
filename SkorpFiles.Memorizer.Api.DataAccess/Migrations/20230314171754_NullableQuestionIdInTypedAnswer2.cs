using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class NullableQuestionIdInTypedAnswer2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rTypedAnswer_rQuestion_QuestionId",
                schema: "memorizer",
                table: "rTypedAnswer");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionId",
                schema: "memorizer",
                table: "rTypedAnswer",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_rTypedAnswer_rQuestion_QuestionId",
                schema: "memorizer",
                table: "rTypedAnswer",
                column: "QuestionId",
                principalSchema: "memorizer",
                principalTable: "rQuestion",
                principalColumn: "QuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rTypedAnswer_rQuestion_QuestionId",
                schema: "memorizer",
                table: "rTypedAnswer");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionId",
                schema: "memorizer",
                table: "rTypedAnswer",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_rTypedAnswer_rQuestion_QuestionId",
                schema: "memorizer",
                table: "rTypedAnswer",
                column: "QuestionId",
                principalSchema: "memorizer",
                principalTable: "rQuestion",
                principalColumn: "QuestionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
