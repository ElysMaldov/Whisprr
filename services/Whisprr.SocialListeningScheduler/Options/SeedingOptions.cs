namespace Whisprr.SocialListeningScheduler.Options;

internal class SeedingOptions
{
    public const string SectionName = "Seeding";

    /// <summary>
    /// Whether to run the database seeder on application startup.
    /// </summary>
    public bool RunOnStartup { get; set; } = false;
}
