using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whisprr.SocialListeningScheduler.Migrations
{
    /// <inheritdoc />
    public partial class MigratePendingAndProcessingToQueued : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Update existing data: pending -> queued, processing -> queued
            migrationBuilder.Sql("UPDATE \"SocialListeningTasks\" SET \"Status\" = 'queued' WHERE \"Status\" = 'pending' OR \"Status\" = 'processing'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Cannot safely revert the data changes
        }
    }
}
