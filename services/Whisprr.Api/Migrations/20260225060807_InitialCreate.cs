using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whisprr.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SocialTopics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Keywords = table.Column<List<string>>(type: "text[]", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialTopics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocialListeningTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ErrorMessage = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    ItemsCollected = table.Column<int>(type: "integer", nullable: false),
                    Platform = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SearchQuery = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialListeningTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialListeningTasks_SocialTopics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "SocialTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    SubscribedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NotificationPreferences = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSubscriptions_SocialTopics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "SocialTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    Platform = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SourceId = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    SourceUrl = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Author = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PostedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CollectedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EngagementData = table.Column<string>(type: "jsonb", nullable: true),
                    RawData = table.Column<string>(type: "jsonb", nullable: true),
                    SentimentScore = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialInfos_SocialListeningTasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "SocialListeningTasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SocialInfos_SocialTopics_TopicId",
                        column: x => x.TopicId,
                        principalTable: "SocialTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SocialInfos_CollectedAt",
                table: "SocialInfos",
                column: "CollectedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SocialInfos_EngagementData",
                table: "SocialInfos",
                column: "EngagementData")
                .Annotation("Npgsql:IndexMethod", "GIN");

            migrationBuilder.CreateIndex(
                name: "IX_SocialInfos_Platform",
                table: "SocialInfos",
                column: "Platform");

            migrationBuilder.CreateIndex(
                name: "IX_SocialInfos_Platform_SourceId",
                table: "SocialInfos",
                columns: new[] { "Platform", "SourceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SocialInfos_TaskId",
                table: "SocialInfos",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialInfos_TopicId",
                table: "SocialInfos",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialInfos_TopicId_CollectedAt",
                table: "SocialInfos",
                columns: new[] { "TopicId", "CollectedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SocialListeningTasks_CreatedAt",
                table: "SocialListeningTasks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SocialListeningTasks_Status",
                table: "SocialListeningTasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SocialListeningTasks_TopicId",
                table: "SocialListeningTasks",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialListeningTasks_TopicId_Status",
                table: "SocialListeningTasks",
                columns: new[] { "TopicId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SocialTopics_CreatedAt",
                table: "SocialTopics",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_TopicId",
                table: "UserSubscriptions",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserSubscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_UserId_TopicId",
                table: "UserSubscriptions",
                columns: new[] { "UserId", "TopicId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SocialInfos");

            migrationBuilder.DropTable(
                name: "UserSubscriptions");

            migrationBuilder.DropTable(
                name: "SocialListeningTasks");

            migrationBuilder.DropTable(
                name: "SocialTopics");
        }
    }
}
