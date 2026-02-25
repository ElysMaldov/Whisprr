using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Whisprr.Contracts.Enums;
using Whisprr.SocialListeningScheduler.Models;

namespace Whisprr.SocialListeningScheduler.Data.Seeding;

internal static class AppDbContextSeeder
{
    private static readonly List<(string[] Keywords, string Language, PlatformType Platform)> DefaultTopics =
    [
        (["climate change", "global warming"], "en", PlatformType.Bluesky),
        (["artificial intelligence", "machine learning"], "en", PlatformType.Bluesky),
        (["renewable energy", "solar power"], "en", PlatformType.Bluesky),
        (["mental health", "wellness"], "en", PlatformType.Mastodon),
        (["remote work", "digital nomad"], "en", PlatformType.Mastodon),
        (["cryptocurrency", "blockchain"], "en", PlatformType.Mastodon),
        (["vegan recipes", "plant based"], "en", PlatformType.Bluesky),
        (["space exploration", "nasa"], "en", PlatformType.Bluesky),
        (["electric vehicles", "ev charging"], "en", PlatformType.Mastodon),
        (["open source", "github"], "en", PlatformType.Mastodon)
    ];

    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        await SeedSocialTopicsAsync(context, cancellationToken);
    }

    private static async Task SeedSocialTopicsAsync(AppDbContext context, CancellationToken cancellationToken)
    {
        // Materialize existing topics and compare in memory
        // Keywords is converted to comma-separated string in DB, so we compare the raw strings
        var existingKeywordSets = await context.SocialTopics
            .Select(st => st.Keywords)
            .ToListAsync(cancellationToken);

        // Normalize for comparison: sort keywords and join them
        var existingNormalized = existingKeywordSets
            .Select(kw => string.Join(",", kw.OrderBy(k => k)))
            .ToHashSet();

        var newTopics = DefaultTopics
            .Where(t => !existingNormalized.Contains(string.Join(",", t.Keywords.OrderBy(k => k))))
            .Select(t => new SocialTopic
            {
                Id = Guid.NewGuid(),
                Keywords = t.Keywords,
                Language = new CultureInfo(t.Language),
                Platform = t.Platform
            })
            .ToList();

        if (newTopics.Count != 0)
        {
            await context.SocialTopics.AddRangeAsync(newTopics, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
