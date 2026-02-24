using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Whisprr.SocialListeningScheduler.Models;

namespace Whisprr.SocialListeningScheduler.Data.Seeding;

internal static class AppDbContextSeeder
{
    private static readonly List<(string Name, string SourceUrl, string IconUrl)> DefaultDataSources =
    [
        ("Bluesky", "https://bsky.app", "https://bsky.app/static/favicon.png"),
        ("Mastodon", "https://mastodon.social", "https://mastodon.social/favicon.ico")
    ];

    private static readonly List<(string[] Keywords, string Language)> BlueskyTopics =
    [
        (["climate change", "global warming"], "en"),
        (["artificial intelligence", "machine learning"], "en"),
        (["renewable energy", "solar power"], "en"),
        (["mental health", "wellness"], "en"),
        (["remote work", "digital nomad"], "en"),
        (["cryptocurrency", "blockchain"], "en"),
        (["vegan recipes", "plant based"], "en"),
        (["space exploration", "nasa"], "en"),
        (["electric vehicles", "ev charging"], "en"),
        (["open source", "github"], "en")
    ];

    private static readonly List<(string[] Keywords, string Language)> MastodonTopics =
    [
        (["privacy", "data protection"], "en"),
        (["fediverse", "decentralized"], "en"),
        (["linux", "ubuntu"], "en"),
        (["indie games", "game dev"], "en"),
        (["photography", "street photo"], "en"),
        (["minimalism", "simple living"], "en"),
        (["cybersecurity", "infosec"], "en"),
        (["open web", "web standards"], "en"),
        (["creative coding", "generative art"], "en"),
        (["music production", "ableton"], "en")
    ];

    public static async Task SeedAsync(AppDbContext context, CancellationToken cancellationToken = default)
    {
        await SeedDataSourcesAsync(context, cancellationToken);
        await SeedSocialTopicsAsync(context, cancellationToken);
    }

    private static async Task SeedDataSourcesAsync(AppDbContext context, CancellationToken cancellationToken)
    {
        var existingSources = await context.DataSources
            .Select(ds => ds.Name)
            .ToListAsync(cancellationToken);

        var newSources = DefaultDataSources
            .Where(ds => !existingSources.Contains(ds.Name))
            .Select(ds => new DataSource
            {
                Id = Guid.NewGuid(),
                Name = ds.Name,
                SourceUrl = ds.SourceUrl,
                IconUrl = ds.IconUrl
            })
            .ToList();

        if (newSources.Count != 0)
        {
            await context.DataSources.AddRangeAsync(newSources, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    private static async Task SeedSocialTopicsAsync(AppDbContext context, CancellationToken cancellationToken)
    {
        var bluesky = await context.DataSources
            .FirstOrDefaultAsync(ds => ds.Name == "Bluesky", cancellationToken);

        var mastodon = await context.DataSources
            .FirstOrDefaultAsync(ds => ds.Name == "Mastodon", cancellationToken);

        if (bluesky is not null)
        {
            await SeedTopicsForDataSourceAsync(context, bluesky.Id, BlueskyTopics, cancellationToken);
        }

        if (mastodon is not null)
        {
            await SeedTopicsForDataSourceAsync(context, mastodon.Id, MastodonTopics, cancellationToken);
        }
    }

    private static async Task SeedTopicsForDataSourceAsync(
        AppDbContext context,
        Guid dataSourceId,
        List<(string[] Keywords, string Language)> topics,
        CancellationToken cancellationToken)
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

        var newTopics = topics
            .Where(t => !existingNormalized.Contains(string.Join(",", t.Keywords.OrderBy(k => k))))
            .Select(t => new SocialTopic
            {
                Id = Guid.NewGuid(),
                Keywords = t.Keywords,
                Language = new CultureInfo(t.Language)
            })
            .ToList();

        if (newTopics.Count != 0)
        {
            await context.SocialTopics.AddRangeAsync(newTopics, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
