namespace Whisprr.Utils.Interfaces;

/// <summary>
/// Used for models needing to track created and updated times
/// </summary>
public interface ITrackableModel
{
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? UpdatedAt { get; set; }

}