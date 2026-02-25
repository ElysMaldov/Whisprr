using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Whisprr.SocialListeningScheduler.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePlatformTypeEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop default value constraints
            migrationBuilder.Sql("ALTER TABLE \"SocialTopics\" ALTER COLUMN \"Platform\" DROP DEFAULT;");
            migrationBuilder.Sql("ALTER TABLE \"SocialListeningTasks\" ALTER COLUMN \"Platform\" DROP DEFAULT;");

            // Update any rows with Twitter or Reddit to use Bluesky
            migrationBuilder.Sql("UPDATE \"SocialTopics\" SET \"Platform\" = 'bluesky' WHERE \"Platform\" IN ('twitter', 'reddit');");
            migrationBuilder.Sql("UPDATE \"SocialListeningTasks\" SET \"Platform\" = 'bluesky' WHERE \"Platform\" IN ('twitter', 'reddit');");

            // Create a new enum type with only Bluesky and Mastodon
            migrationBuilder.Sql("CREATE TYPE platform_type_new AS ENUM ('bluesky', 'mastodon');");

            // Change columns to use the new enum type
            migrationBuilder.Sql("ALTER TABLE \"SocialTopics\" ALTER COLUMN \"Platform\" TYPE platform_type_new USING \"Platform\"::text::platform_type_new;");
            migrationBuilder.Sql("ALTER TABLE \"SocialListeningTasks\" ALTER COLUMN \"Platform\" TYPE platform_type_new USING \"Platform\"::text::platform_type_new;");

            // Drop the old enum type
            migrationBuilder.Sql("DROP TYPE platform_type;");

            // Rename the new enum type to the original name
            migrationBuilder.Sql("ALTER TYPE platform_type_new RENAME TO platform_type;");

            // Re-add default value constraints
            migrationBuilder.Sql("ALTER TABLE \"SocialTopics\" ALTER COLUMN \"Platform\" SET DEFAULT 'bluesky'::platform_type;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop default value constraints
            migrationBuilder.Sql("ALTER TABLE \"SocialTopics\" ALTER COLUMN \"Platform\" DROP DEFAULT;");
            migrationBuilder.Sql("ALTER TABLE \"SocialListeningTasks\" ALTER COLUMN \"Platform\" DROP DEFAULT;");

            // Create the old enum type with all values
            migrationBuilder.Sql("CREATE TYPE platform_type_old AS ENUM ('bluesky', 'mastodon', 'twitter', 'reddit');");

            // Change columns back to the old enum type
            migrationBuilder.Sql("ALTER TABLE \"SocialTopics\" ALTER COLUMN \"Platform\" TYPE platform_type_old USING \"Platform\"::text::platform_type_old;");
            migrationBuilder.Sql("ALTER TABLE \"SocialListeningTasks\" ALTER COLUMN \"Platform\" TYPE platform_type_old USING \"Platform\"::text::platform_type_old;");

            // Drop the new enum type
            migrationBuilder.Sql("DROP TYPE platform_type;");

            // Rename back to original name
            migrationBuilder.Sql("ALTER TYPE platform_type_old RENAME TO platform_type;");

            // Re-add default value constraints
            migrationBuilder.Sql("ALTER TABLE \"SocialTopics\" ALTER COLUMN \"Platform\" SET DEFAULT 'bluesky'::platform_type;");
        }
    }
}
