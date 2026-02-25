using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Whisprr.Contracts.Enums;

#nullable disable

namespace Whisprr.SocialListeningScheduler.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDataSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SocialListeningTasks_DataSources_SourcePlatformId",
                table: "SocialListeningTasks");

            migrationBuilder.DropTable(
                name: "DataSources");

            migrationBuilder.DropIndex(
                name: "IX_SocialListeningTasks_SourcePlatformId",
                table: "SocialListeningTasks");

            migrationBuilder.DropColumn(
                name: "SourcePlatformId",
                table: "SocialListeningTasks");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:platform_type", "bluesky,mastodon,twitter,reddit")
                .Annotation("Npgsql:Enum:task_progress_status", "pending,processing,queued,success,failed")
                .OldAnnotation("Npgsql:Enum:task_progress_status", "pending,processing,queued,success,failed");

            migrationBuilder.AddColumn<PlatformType>(
                name: "Platform",
                table: "SocialListeningTasks",
                type: "platform_type",
                nullable: false,
                defaultValue: PlatformType.Bluesky);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Platform",
                table: "SocialListeningTasks");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:task_progress_status", "pending,processing,queued,success,failed")
                .OldAnnotation("Npgsql:Enum:platform_type", "bluesky,mastodon,twitter,reddit")
                .OldAnnotation("Npgsql:Enum:task_progress_status", "pending,processing,queued,success,failed");

            migrationBuilder.AddColumn<Guid>(
                name: "SourcePlatformId",
                table: "SocialListeningTasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "DataSources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IconUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SourceUrl = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSources", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SocialListeningTasks_SourcePlatformId",
                table: "SocialListeningTasks",
                column: "SourcePlatformId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSources_Name",
                table: "DataSources",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SocialListeningTasks_DataSources_SourcePlatformId",
                table: "SocialListeningTasks",
                column: "SourcePlatformId",
                principalTable: "DataSources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
