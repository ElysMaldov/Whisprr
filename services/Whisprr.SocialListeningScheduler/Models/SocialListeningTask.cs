using System.ComponentModel.DataAnnotations.Schema;
using Whisprr.Contracts.Enums;
using Whisprr.Utils.Interfaces;

namespace Whisprr.SocialListeningScheduler.Models;

internal class SocialListeningTask : ITrackableModel
{
  public Guid Id { get; set; }
  public DateTimeOffset CreatedAt { get; set; }
  public DateTimeOffset? UpdatedAt { get; set; }

  public TaskProgressStatus Status { get; set; }

  public Guid SocialTopicId { get; set; }
  
  [ForeignKey(nameof(SocialTopicId))]
  public SocialTopic SocialTopic { get; set; } = null!; // Use dammit to avoid this field being nullable by compiler, but will be populated by EF Core. Kind of like late in dart.

  public Guid SourcePlatformId { get; set; }
  
  [ForeignKey(nameof(SourcePlatformId))]
  public DataSource DataSource { get; set; } = null!;
  
  [NotMapped]
  public string Query
  {
    get => string.Join(" ", SocialTopic.Keywords);
  }
}