using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Whisprr.Contracts.Enums;

#nullable disable

namespace Whisprr.SocialListeningScheduler.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:task_progress_status", "processing,success,failed");

            migrationBuilder.CreateTable(
                name: "DataSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SourceUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    IconUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocialTopics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Keywords = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false)
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
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<TaskProgressStatus>(type: "task_progress_status", nullable: false),
                    SocialTopicId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourcePlatformId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialListeningTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialListeningTasks_DataSources_SourcePlatformId",
                        column: x => x.SourcePlatformId,
                        principalTable: "DataSources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SocialListeningTasks_SocialTopics_SocialTopicId",
                        column: x => x.SocialTopicId,
                        principalTable: "SocialTopics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataSources_Name",
                table: "DataSources",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SocialListeningTasks_SocialTopicId",
                table: "SocialListeningTasks",
                column: "SocialTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialListeningTasks_SourcePlatformId",
                table: "SocialListeningTasks",
                column: "SourcePlatformId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SocialListeningTasks");

            migrationBuilder.DropTable(
                name: "DataSources");

            migrationBuilder.DropTable(
                name: "SocialTopics");
        }
    }
}
