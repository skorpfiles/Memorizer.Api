﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SkorpFiles.Memorizer.Api.DataAccess;

#nullable disable

namespace SkorpFiles.Memorizer.Api.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230807091649_AddedDbContextFactory")]
    partial class AddedDbContextFactory
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("IdentityUser");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.AuthenticationCache", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Key");

                    b.ToTable("jAuthenticationCache", "memorizer");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.EntityLabel", b =>
                {
                    b.Property<Guid>("EntityLabelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("EntityType")
                        .HasColumnType("int");

                    b.Property<Guid>("LabelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("LabelNumber")
                        .HasColumnType("int");

                    b.Property<DateTime>("ObjectCreationTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("ObjectCreationTime");

                    b.Property<Guid?>("ParentLabelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("QuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("QuestionnaireId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("EntityLabelId");

                    b.HasIndex("LabelId");

                    b.HasIndex("QuestionId");

                    b.HasIndex("QuestionnaireId");

                    b.ToTable("nnEntityLabel", "memorizer");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.EventLog", b =>
                {
                    b.Property<Guid>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("EventMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EventTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("EventTime");

                    b.HasKey("EventId");

                    b.ToTable("jEventLog", "memorizer");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.Label", b =>
                {
                    b.Property<Guid>("LabelId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("LabelCode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LabelCode"));

                    b.Property<string>("LabelName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("ObjectCreationTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("ObjectCreationTime");

                    b.Property<bool>("ObjectIsRemoved")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ObjectRemovalTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("ObjectRemovalTime");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LabelId");

                    b.HasIndex("OwnerId");

                    b.ToTable("sLabel", "memorizer");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.Question", b =>
                {
                    b.Property<Guid>("QuestionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ObjectCreationTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("ObjectCreationTime");

                    b.Property<bool>("ObjectIsRemoved")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ObjectRemovalTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("ObjectRemovalTime");

                    b.Property<int>("QuestionEstimatedTrainingTimeSeconds")
                        .HasColumnType("int");

                    b.Property<bool>("QuestionIsEnabled")
                        .HasColumnType("bit");

                    b.Property<bool>("QuestionIsFixed")
                        .HasColumnType("bit");

                    b.Property<int>("QuestionQuestionnaireCode")
                        .HasColumnType("int");

                    b.Property<string>("QuestionReference")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("QuestionText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("QuestionType")
                        .HasColumnType("int");

                    b.Property<string>("QuestionUntypedAnswer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("QuestionnaireId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("QuestionId");

                    b.HasIndex("QuestionnaireId");

                    b.ToTable("rQuestion", "memorizer");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.QuestionUser", b =>
                {
                    b.Property<Guid>("QuestionUserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ObjectCreationTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("ObjectCreationTime");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("QuestionUserIsNew")
                        .HasColumnType("bit");

                    b.Property<int>("QuestionUserPenaltyPoints")
                        .HasColumnType("int");

                    b.Property<int>("QuestionUserRating")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("QuestionUserId");

                    b.HasIndex("QuestionId");

                    b.HasIndex("UserId");

                    b.ToTable("nnQuestionUser", "memorizer");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.Questionnaire", b =>
                {
                    b.Property<Guid>("QuestionnaireId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ObjectCreationTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("ObjectCreationTime");

                    b.Property<bool>("ObjectIsRemoved")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ObjectRemovalTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("ObjectRemovalTime");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("QuestionnaireAvailability")
                        .HasColumnType("int");

                    b.Property<int>("QuestionnaireCode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("QuestionnaireCode"));

                    b.Property<string>("QuestionnaireName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("QuestionnaireId");

                    b.HasIndex("OwnerId");

                    b.ToTable("rQuestionnaire", "memorizer");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.Training", b =>
                {
                    b.Property<Guid>("TrainingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ObjectCreationTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("ObjectCreationTime");

                    b.Property<bool>("ObjectIsRemoved")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ObjectRemovalTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("ObjectRemovalTime");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("TrainingLastTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("TrainingLastTime");

                    b.Property<int>("TrainingLengthType")
                        .HasColumnType("int");

                    b.Property<string>("TrainingName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TrainingQuestionsCount")
                        .HasColumnType("int");

                    b.Property<int>("TrainingTimeMinutes")
                        .HasColumnType("int");

                    b.HasKey("TrainingId");

                    b.HasIndex("OwnerId");

                    b.ToTable("rTraining", "memorizer");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.TrainingQuestionnaire", b =>
                {
                    b.Property<Guid>("TrainingQuestionnaireId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ObjectCreationTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("ObjectCreationTime");

                    b.Property<Guid>("QuestionnaireId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TrainingId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("TrainingQuestionnaireId");

                    b.HasIndex("QuestionnaireId");

                    b.HasIndex("TrainingId");

                    b.ToTable("nnTrainingQuestionnaire", "memorizer");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.TrainingResult", b =>
                {
                    b.Property<Guid>("TrainingResultId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("TrainingResultAnswerIsCorrect")
                        .HasColumnType("bit");

                    b.Property<int>("TrainingResultPenaltyPoints")
                        .HasColumnType("int");

                    b.Property<bool>("TrainingResultQuestionHasBeenNew")
                        .HasColumnType("bit");

                    b.Property<Guid>("TrainingResultQuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("TrainingResultRating")
                        .HasColumnType("int");

                    b.Property<DateTime>("TrainingResultRecordingTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("TrainingResultTimeMilliseconds")
                        .HasColumnType("int");

                    b.Property<string>("TrainingResultUntypedAnswer")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TrainingResultUserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("TrainingResultId");

                    b.HasIndex("TrainingResultQuestionId");

                    b.HasIndex("TrainingResultUserId");

                    b.ToTable("jTrainingResult", "memorizer");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.TrainingResultTypedAnswer", b =>
                {
                    b.Property<Guid>("TrtaId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("TrainingResultId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TrtaAnswer")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TrtaIsCorrect")
                        .HasColumnType("bit");

                    b.HasKey("TrtaId");

                    b.HasIndex("TrainingResultId");

                    b.ToTable("jTrainingResultTypedAnswer", "memorizer");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.TypedAnswer", b =>
                {
                    b.Property<Guid>("TypedAnswerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ObjectCreationTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("ObjectCreationTime");

                    b.Property<bool>("ObjectIsRemoved")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ObjectRemovalTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("ObjectRemovalTime");

                    b.Property<Guid?>("QuestionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TypedAnswerText")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("TypedAnswerId");

                    b.HasIndex("QuestionId");

                    b.ToTable("rTypedAnswer", "memorizer");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.UserActivity", b =>
                {
                    b.Property<Guid>("UserActivityId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ObjectCreationTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("ObjectCreationTime");

                    b.Property<bool>("ObjectIsRemoved")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("ObjectRemovalTimeUtc")
                        .HasColumnType("datetime2")
                        .HasColumnName("ObjectRemovalTime");

                    b.Property<int>("UserCode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserCode"));

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("UserIsEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserActivityId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("rUserActivity", "memorizer");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.ApplicationUser", b =>
                {
                    b.HasBaseType("Microsoft.AspNetCore.Identity.IdentityUser");

                    b.HasDiscriminator().HasValue("ApplicationUser");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.EntityLabel", b =>
                {
                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.Label", "Label")
                        .WithMany("EntitiesForLabel")
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.Question", "Question")
                        .WithMany("LabelsForQuestion")
                        .HasForeignKey("QuestionId");

                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.Questionnaire", "Questionnaire")
                        .WithMany("LabelsForQuestionnaire")
                        .HasForeignKey("QuestionnaireId");

                    b.Navigation("Label");

                    b.Navigation("Question");

                    b.Navigation("Questionnaire");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.Label", b =>
                {
                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.ApplicationUser", "Owner")
                        .WithMany("LabelsThatUserOwns")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.Question", b =>
                {
                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.Questionnaire", "Questionnaire")
                        .WithMany("Questions")
                        .HasForeignKey("QuestionnaireId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Questionnaire");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.QuestionUser", b =>
                {
                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.Question", "Question")
                        .WithMany("UsersForQuestion")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.ApplicationUser", "User")
                        .WithMany("QuestionsForUser")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Question");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.Questionnaire", b =>
                {
                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.ApplicationUser", "Owner")
                        .WithMany("QuestionnairesThatUserOwns")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.Training", b =>
                {
                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.ApplicationUser", "Owner")
                        .WithMany("TrainingsForUser")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.TrainingQuestionnaire", b =>
                {
                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.Questionnaire", "Questionnaire")
                        .WithMany("TrainingsForQuestionnaire")
                        .HasForeignKey("QuestionnaireId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.Training", "Training")
                        .WithMany("QuestionnairesForTraining")
                        .HasForeignKey("TrainingId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Questionnaire");

                    b.Navigation("Training");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.TrainingResult", b =>
                {
                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.Question", "TrainingResultQuestion")
                        .WithMany("TrainingResults")
                        .HasForeignKey("TrainingResultQuestionId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.ApplicationUser", "TrainingResultUser")
                        .WithMany("TrainingResultsForUser")
                        .HasForeignKey("TrainingResultUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("TrainingResultQuestion");

                    b.Navigation("TrainingResultUser");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.TrainingResultTypedAnswer", b =>
                {
                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.TrainingResult", "TrainingResult")
                        .WithMany("TypedAnswers")
                        .HasForeignKey("TrainingResultId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("TrainingResult");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.TypedAnswer", b =>
                {
                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.Question", "Question")
                        .WithMany("TypedAnswers")
                        .HasForeignKey("QuestionId");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.UserActivity", b =>
                {
                    b.HasOne("SkorpFiles.Memorizer.Api.DataAccess.Models.ApplicationUser", "ApplicationUser")
                        .WithOne("UserActivity")
                        .HasForeignKey("SkorpFiles.Memorizer.Api.DataAccess.Models.UserActivity", "UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ApplicationUser");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.Label", b =>
                {
                    b.Navigation("EntitiesForLabel");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.Question", b =>
                {
                    b.Navigation("LabelsForQuestion");

                    b.Navigation("TrainingResults");

                    b.Navigation("TypedAnswers");

                    b.Navigation("UsersForQuestion");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.Questionnaire", b =>
                {
                    b.Navigation("LabelsForQuestionnaire");

                    b.Navigation("Questions");

                    b.Navigation("TrainingsForQuestionnaire");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.Training", b =>
                {
                    b.Navigation("QuestionnairesForTraining");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.TrainingResult", b =>
                {
                    b.Navigation("TypedAnswers");
                });

            modelBuilder.Entity("SkorpFiles.Memorizer.Api.DataAccess.Models.ApplicationUser", b =>
                {
                    b.Navigation("LabelsThatUserOwns");

                    b.Navigation("QuestionnairesThatUserOwns");

                    b.Navigation("QuestionsForUser");

                    b.Navigation("TrainingResultsForUser");

                    b.Navigation("TrainingsForUser");

                    b.Navigation("UserActivity");
                });
#pragma warning restore 612, 618
        }
    }
}
