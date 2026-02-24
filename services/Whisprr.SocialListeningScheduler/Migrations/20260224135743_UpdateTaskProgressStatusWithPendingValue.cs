using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whisprr.SocialListeningScheduler.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskProgressStatusWithPendingValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:task_progress_status", "pending,processing,success,failed")
                .OldAnnotation("Npgsql:Enum:task_progress_status", "processing,success,failed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:task_progress_status", "processing,success,failed")
                .OldAnnotation("Npgsql:Enum:task_progress_status", "pending,processing,success,failed");
        }
    }
}
