using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whisprr.SocialListeningScheduler.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaskProgressStatusAddQueued : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add the enum value only if it doesn't already exist
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM pg_enum 
                        WHERE enumlabel = 'queued' 
                        AND enumtypid = (SELECT oid FROM pg_type WHERE typname = 'task_progress_status')
                    ) THEN
                        ALTER TYPE task_progress_status ADD VALUE 'queued' AFTER 'processing';
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Cannot safely remove enum values in PostgreSQL
        }
    }
}
