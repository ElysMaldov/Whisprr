using System.Threading.Channels;
using MassTransit;
using Whisprr.Contracts.Events;
using Whisprr.SocialScouter.Models;

namespace Whisprr.SocialScouter.Modules.Workers;

/// <summary>
/// Worker that consumes SocialInfo from the channel for further processing.
/// Only one instance of this worker should run (SingleReader = true on the channel).
/// </summary>
internal partial class SocialInfoProcessorWorker(
    ILogger<SocialInfoProcessorWorker> logger,
    ChannelReader<SocialInfo> socialInfoChannelReader,
    IPublishEndpoint publishEndpoint) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogWorkerStarted(logger);

        await foreach (var socialInfo in socialInfoChannelReader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessSocialInfoAsync(socialInfo, stoppingToken);
            }
            catch (Exception ex)
            {
                LogProcessingFailed(logger, ex, socialInfo.Id);
            }
        }
    }

    private async Task ProcessSocialInfoAsync(SocialInfo socialInfo, CancellationToken stoppingToken)
    {
        LogProcessingSocialInfo(logger, socialInfo.Id);

        // Publish the SocialInfoCreated event
        var socialInfoCreated = new SocialInfoCreated
        {
            InfoId = socialInfo.Id,
            CreatedAt = socialInfo.CreatedAt,
            Title = socialInfo.Title,
            Content = socialInfo.Content,
            OriginalUrl = socialInfo.OriginalUrl,
            OriginalId = socialInfo.OriginalId,
            Platform = socialInfo.Platform,
            GeneratedFromTaskId = socialInfo.GeneratedFromTaskId
        };

        await publishEndpoint.Publish(socialInfoCreated, stoppingToken);

        var contentPreview = socialInfo.Content?[..Math.Min(50, socialInfo.Content?.Length ?? 0)];
        LogSocialInfoProcessed(logger, socialInfo.Id, contentPreview);
    }

    // LoggerMessage source-generated methods to avoid boxing
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "SocialInfoProcessorWorker started. Waiting for SocialInfo...")]
    static partial void LogWorkerStarted(ILogger<SocialInfoProcessorWorker> logger);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Processing SocialInfo {SocialInfoId}")]
    static partial void LogProcessingSocialInfo(ILogger<SocialInfoProcessorWorker> logger, Guid socialInfoId);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Processed and published SocialInfo {SocialInfoId}: {ContentPreview}")]
    static partial void LogSocialInfoProcessed(ILogger<SocialInfoProcessorWorker> logger, Guid socialInfoId, string? contentPreview);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Failed to process SocialInfo {SocialInfoId}")]
    static partial void LogProcessingFailed(ILogger<SocialInfoProcessorWorker> logger, Exception ex, Guid socialInfoId);
}
