using Whisprr.SocialScouter.Models;

namespace Whisprr.SocialScouter.Modules.SocialListener;

/// <summary>
/// I still use an interface here for easier Dependency Injection in our program
/// and tests.
/// </summary>
internal interface ISocialListener
{
  public Task<SocialInfo[]> Search(SocialListeningTask task);
}