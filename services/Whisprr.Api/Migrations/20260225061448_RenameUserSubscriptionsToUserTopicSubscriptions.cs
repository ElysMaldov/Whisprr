using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whisprr.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserSubscriptionsToUserTopicSubscriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserSubscriptions",
                newName: "UserTopicSubscriptions");

            migrationBuilder.RenameIndex(
                name: "IX_UserSubscriptions_TopicId",
                table: "UserTopicSubscriptions",
                newName: "IX_UserTopicSubscriptions_TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSubscriptions_UserId",
                table: "UserTopicSubscriptions",
                newName: "IX_UserTopicSubscriptions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserSubscriptions_UserId_TopicId",
                table: "UserTopicSubscriptions",
                newName: "IX_UserTopicSubscriptions_UserId_TopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UserTopicSubscriptions",
                newName: "UserSubscriptions");

            migrationBuilder.RenameIndex(
                name: "IX_UserTopicSubscriptions_TopicId",
                table: "UserSubscriptions",
                newName: "IX_UserSubscriptions_TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTopicSubscriptions_UserId",
                table: "UserSubscriptions",
                newName: "IX_UserSubscriptions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTopicSubscriptions_UserId_TopicId",
                table: "UserSubscriptions",
                newName: "IX_UserSubscriptions_UserId_TopicId");
        }
    }
}
