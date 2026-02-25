using Microsoft.EntityFrameworkCore.Migrations;
using Whisprr.Contracts.Enums;

#nullable disable

namespace Whisprr.SocialListeningScheduler.Migrations
{
    /// <inheritdoc />
    public partial class AddPlatformToSocialTopic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<PlatformType>(
                name: "Platform",
                table: "SocialTopics",
                type: "platform_type",
                nullable: false,
                defaultValue: PlatformType.Bluesky);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Platform",
                table: "SocialTopics");
        }
    }
}
