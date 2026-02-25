namespace Whisprr.SocialListeningScheduler.Modules.SocialListeningTaskPublisher;

internal static class SocialListeningTaskPublisherExtensions
{
  public static IHostApplicationBuilder AddSocialListeningTaskPublisher(this IHostApplicationBuilder builder)
  {
    builder.Services.AddScoped<ISocialListeningTaskPublisher, SocialListeningTaskPublisher>();

    return builder;
  }


}