using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SkorpFiles.Memorizer.Api.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "memorizer");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rQuestionnaire",
                schema: "memorizer",
                columns: table => new
                {
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireCode = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionnaireName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionnaireAvailability = table.Column<int>(type: "int", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ObjectCreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ObjectIsRemoved = table.Column<bool>(type: "bit", nullable: false),
                    ObjectRemovalTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rQuestionnaire", x => x.QuestionnaireId);
                    table.ForeignKey(
                        name: "FK_rQuestionnaire_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rUserActivity",
                schema: "memorizer",
                columns: table => new
                {
                    UserActivityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserCode = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserIsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ObjectCreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ObjectIsRemoved = table.Column<bool>(type: "bit", nullable: false),
                    ObjectRemovalTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rUserActivity", x => x.UserActivityId);
                    table.ForeignKey(
                        name: "FK_rUserActivity_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sLabel",
                schema: "memorizer",
                columns: table => new
                {
                    LabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: false),
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

            migrationBuilder.CreateTable(
                name: "rQuestion",
                schema: "memorizer",
                columns: table => new
                {
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionUntypedAnswer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionIsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    QuestionReference = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuestionIsFixed = table.Column<bool>(type: "bit", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionQuestionnaireCode = table.Column<int>(type: "int", nullable: false),
                    QuestionAddedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ObjectCreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ObjectIsRemoved = table.Column<bool>(type: "bit", nullable: false),
                    ObjectRemovalTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rQuestion", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_rQuestion_rQuestionnaire_QuestionnaireId",
                        column: x => x.QuestionnaireId,
                        principalSchema: "memorizer",
                        principalTable: "rQuestionnaire",
                        principalColumn: "QuestionnaireId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "jEventLog",
                schema: "memorizer",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EventTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventQuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EventQuestionIsNew = table.Column<bool>(type: "bit", nullable: true),
                    EventTypedAnswers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventResultRating = table.Column<int>(type: "int", nullable: true),
                    EventResultPenaltyPoints = table.Column<int>(type: "int", nullable: true),
                    EventMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_jEventLog", x => x.EventId);
                    table.ForeignKey(
                        name: "FK_jEventLog_rQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "memorizer",
                        principalTable: "rQuestion",
                        principalColumn: "QuestionId");
                });

            migrationBuilder.CreateTable(
                name: "nnEntityLabel",
                schema: "memorizer",
                columns: table => new
                {
                    EntityLabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionnaireId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentLabelId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LabelNumber = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    ObjectCreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "nnQuestionUser",
                schema: "memorizer",
                columns: table => new
                {
                    QuestionUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionUserIsNew = table.Column<bool>(type: "bit", nullable: false),
                    QuestionUserRating = table.Column<int>(type: "int", nullable: false),
                    QuestionUserPenaltyPoints = table.Column<int>(type: "int", nullable: false),
                    ObjectCreationTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nnQuestionUser", x => x.QuestionUserId);
                    table.ForeignKey(
                        name: "FK_nnQuestionUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_nnQuestionUser_rQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "memorizer",
                        principalTable: "rQuestion",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rTypedAnswer",
                schema: "memorizer",
                columns: table => new
                {
                    TypedAnswerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TypedAnswerQuestionId = table.Column<int>(type: "int", nullable: false),
                    TypedAnswerText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObjectCreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ObjectIsRemoved = table.Column<bool>(type: "bit", nullable: false),
                    ObjectRemovalTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rTypedAnswer", x => x.TypedAnswerId);
                    table.ForeignKey(
                        name: "FK_rTypedAnswer_rQuestion_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "memorizer",
                        principalTable: "rQuestion",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_jEventLog_QuestionId",
                schema: "memorizer",
                table: "jEventLog",
                column: "QuestionId");

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

            migrationBuilder.CreateIndex(
                name: "IX_nnQuestionUser_QuestionId",
                schema: "memorizer",
                table: "nnQuestionUser",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_nnQuestionUser_UserId",
                schema: "memorizer",
                table: "nnQuestionUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_rQuestion_QuestionnaireId",
                schema: "memorizer",
                table: "rQuestion",
                column: "QuestionnaireId");

            migrationBuilder.CreateIndex(
                name: "IX_rQuestionnaire_OwnerId",
                schema: "memorizer",
                table: "rQuestionnaire",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_rTypedAnswer_QuestionId",
                schema: "memorizer",
                table: "rTypedAnswer",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_rUserActivity_UserId",
                schema: "memorizer",
                table: "rUserActivity",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sLabel_OwnerId",
                schema: "memorizer",
                table: "sLabel",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "jEventLog",
                schema: "memorizer");

            migrationBuilder.DropTable(
                name: "nnEntityLabel",
                schema: "memorizer");

            migrationBuilder.DropTable(
                name: "nnQuestionUser",
                schema: "memorizer");

            migrationBuilder.DropTable(
                name: "rTypedAnswer",
                schema: "memorizer");

            migrationBuilder.DropTable(
                name: "rUserActivity",
                schema: "memorizer");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "sLabel",
                schema: "memorizer");

            migrationBuilder.DropTable(
                name: "rQuestion",
                schema: "memorizer");

            migrationBuilder.DropTable(
                name: "rQuestionnaire",
                schema: "memorizer");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
