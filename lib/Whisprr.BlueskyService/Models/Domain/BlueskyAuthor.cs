namespace Whisprr.BlueskyService.Models.Domain;

public readonly struct BlueskyAuthor(
    string dId,
    string handle,
    string displayName,
    string avatar,
    DateTimeOffset createdAt)
{
    public string DId { get; } = dId;
    public string Handle { get; } = handle;
    public string DisplayName { get; } = displayName;
    public string Avatar { get; } = avatar;
    public DateTimeOffset CreatedAt { get; } = createdAt;
}
